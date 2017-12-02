package org.lotusconnect.tcp;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.UnknownHostException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Base64;
import java.util.EnumSet;
import java.util.Timer;
import java.util.TimerTask;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;

import org.apache.log4j.Logger;
import org.lotusconnect.data.BSONConvert;
import org.lotusconnect.data.LPublicKey;
import org.lotusconnect.data.LRequest;
import org.lotusconnect.data.LResponse;
import org.lotusconnect.data.CThumbprint;
import org.lotusconnect.data.LAESInfo;
import org.lotusconnect.data.LocalConfig;
import org.lotusconnect.data.system.SystemInfo;
import org.lotusconnect.main.Program;
import org.lotusconnect.plugin.PluginStore;

public class RConnection {

	private static final int RESPONSE_BUFFER_SIZE = 1024;
	private static final int HEARTBEAT_POLL_TIME = 1000 * 30;

	private static Logger LOGGER = Logger.getLogger(RConnection.class);

	private int _port;
	private String _hostname;
	private Socket _socket;
	private DataOutputStream _outgoingStream;
	private DataInputStream _incomingStream;

	private RCommandProcessor _processor;

	private BSONConvert<LResponse> _bsonResponse;
	private BSONConvert<LRequest> _bsonRequest;

	public RConnection(String hostname, int port) {
		_port = port;
		_hostname = hostname;
		_processor = new RCommandProcessor(this);
		_bsonResponse = new BSONConvert<LResponse>();
		_bsonRequest = new BSONConvert<LRequest>();
	}

	public void connect() throws UnknownHostException, IOException {
		_socket = new Socket(_hostname, _port);
		_outgoingStream = new DataOutputStream(_socket.getOutputStream());
		_incomingStream = new DataInputStream(_socket.getInputStream());
		handshake();
	}

	public void start() throws IOException, IllegalBlockSizeException, BadPaddingException {
		TimerTask poll = new RPoll(this);
		Timer timer = new Timer();
		timer.schedule(poll, 0, HEARTBEAT_POLL_TIME);
		keepListening();
		_outgoingStream.flush();
		_outgoingStream.close();
	}

	private void handshake() {
		try {
			LPacket publicKeyHandshake = waitForResponse();
			LPublicKey publicKey = (new BSONConvert<LPublicKey>()).fromBytes(publicKeyHandshake.getPackagedData(),
					LPublicKey.class);
			LCipher.setRemotePublicKey(publicKey);

			{
				byte[] bsonPublicKey = (new BSONConvert<LPublicKey>()).toBytes(LCipher.getLocalPublicKey());
				EnumSet<LMetadata> metadata = EnumSet.noneOf(LMetadata.class);
				metadata.add(LMetadata.HANDSHAKE);
				LPacket packet = new LPacket(bsonPublicKey, metadata);
				sendPacket(packet);
			}

			LPacket aesHandshake = waitForResponse();
			byte[] aesHandshakePackage = LCipher.localDecrypt(aesHandshake.getPackagedData());
			LAESInfo remoteAESInfo = (new BSONConvert<LAESInfo>()).fromBytes(aesHandshakePackage, LAESInfo.class);
			LCipher.loadRemoteAESInfo(remoteAESInfo);

			{
				byte[] aesData = (new BSONConvert<LAESInfo>()).toBytes(LCipher.getLocalAESInfo());
				EnumSet<LMetadata> metadata = EnumSet.noneOf(LMetadata.class);
				metadata.add(LMetadata.HANDSHAKE);
				metadata.add(LMetadata.ENCRYPTED);
				byte[] encrypted = LCipher.remoteEncrypt(aesData);
				LPacket packet = new LPacket(encrypted, metadata);
				sendPacket(packet);
			}

			String identifier = Base64.getEncoder().encodeToString(LocalConfig.loadConfig().getCIdentifier());
			CThumbprint thumbprint = new CThumbprint(identifier, Program.APPLICATION_VERSION, "local", Program.AUTH,
					SystemInfo.getHostname(), PluginStore.getInstalledPluginDefinitions());

			{
				byte[] bsonThumbprint = (new BSONConvert<CThumbprint>()).toBytes(thumbprint);
				EnumSet<LMetadata> metadata = EnumSet.noneOf(LMetadata.class);
				metadata.add(LMetadata.HANDSHAKE);
				metadata.add(LMetadata.ENCRYPTED);
				byte[] encrypted = LCipher.remoteAESEncrypt(bsonThumbprint);
				LPacket packet = new LPacket(encrypted, metadata);
				sendPacket(packet);
			}
		} catch (Exception e) {
			LOGGER.fatal("Failed handshake! " + e.getMessage());
		}
	}

	private void keepListening() throws IOException, IllegalBlockSizeException, BadPaddingException {
		while (true) {
			LPacket packet = waitForResponse();
			if (packet != null) {
				EnumSet<LMetadata> metadata = LMetadata.fromByte(packet.getMetadata());
				byte[] packaged = packet.getPackagedData();
				if (metadata.contains(LMetadata.HANDSHAKE)) {
					continue;
				}
				if (metadata.contains(LMetadata.ENCRYPTED)) {
					packaged = LCipher.localAESDecrypt(packaged);
				}
				if (metadata.contains(LMetadata.REQUEST)) {
					LRequest request = _bsonRequest.fromBytes(packaged, LRequest.class);
					_processor.processRequest(request);
				} else if (metadata.contains(LMetadata.RESPONSE)) {
					LResponse response = _bsonResponse.fromBytes(packaged, LResponse.class);
					_processor.processResponse(response);
				}
			} else {
				throw new IOException("Malformed packet!");
			}
		}
	}

	public void sendRequest(LRequest request, EnumSet<LMetadata> metadata)
			throws IllegalBlockSizeException, BadPaddingException, IOException {
		byte[] data = _bsonRequest.toBytes(request);
		byte[] encrypted = LCipher.remoteAESEncrypt(data);
		metadata.add(LMetadata.ENCRYPTED);
		metadata.add(LMetadata.REQUEST);
		LPacket packet = new LPacket(encrypted, metadata);
		sendPacket(packet);
	}

	public void sendResponse(LResponse response, EnumSet<LMetadata> metadata)
			throws IOException, IllegalBlockSizeException, BadPaddingException {
		byte[] data = _bsonResponse.toBytes(response);
		byte[] encrypted = LCipher.remoteAESEncrypt(data);
		metadata.add(LMetadata.ENCRYPTED);
		metadata.add(LMetadata.RESPONSE);
		LPacket packet = new LPacket(encrypted, metadata);
		sendPacket(packet);
	}

	public void sendPacket(LPacket packet) throws IOException {
		for (int i = 0; i < packet.getPacketLength(); i++) {
			_outgoingStream.writeByte(packet.getPacket()[i]);
		}
	}

	public LPacket waitForResponse() {
		try {
			byte[] header = new byte[LPacket.LENGTH_LENGTH];
			for (int i = 0; i < header.length; i++) {
				header[i] = (byte) _incomingStream.readByte();
			}
			int length = ByteBuffer.wrap(header).order(ByteOrder.LITTLE_ENDIAN).getInt();
			byte metadata = (byte) _incomingStream.readByte();
			byte[] data = new byte[length];
			int bytesRead = 0;
			while (bytesRead < length - LPacket.METADATA_LENGTH) {
				int read = _incomingStream.read(data, bytesRead,
						Math.min(data.length - bytesRead, RESPONSE_BUFFER_SIZE));
				bytesRead += read;
			}
			return new LPacket(header, metadata, data);
		} catch (Exception e) {
			LOGGER.warn("Error occurred waiting for response: " + e.getMessage());
			return null;
		}
	}

	public DataInputStream getIncomingStream() {
		return _incomingStream;
	}

	public DataOutputStream getOutgoingStream() {
		return _outgoingStream;
	}

	public String toString() {
		return _hostname + ":" + _port;
	}
}
