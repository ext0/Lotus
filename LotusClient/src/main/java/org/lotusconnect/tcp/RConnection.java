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

import org.apache.log4j.Logger;
import org.lotusconnect.data.BSONConvert;
import org.lotusconnect.data.LPublicKey;
import org.lotusconnect.data.CThumbprint;
import org.lotusconnect.data.LAESInfo;
import org.lotusconnect.data.LocalConfig;
import org.lotusconnect.data.system.SystemInfo;
import org.lotusconnect.main.Program;

public class RConnection {

	private static final int RESPONSE_BUFFER_SIZE = 1024;
	private static final int HEARTBEAT_POLL_TIME = 1000 * 30;

	private static Logger LOGGER = Logger.getLogger(RConnection.class);

	private int _port;
	private String _hostname;
	private Socket _socket;
	private DataOutputStream _outgoingStream;
	private DataInputStream _incomingStream;

	public RConnection(String hostname, int port) {
		_port = port;
		_hostname = hostname;
	}

	public void connect() throws UnknownHostException, IOException {
		_socket = new Socket(_hostname, _port);
		_outgoingStream = new DataOutputStream(_socket.getOutputStream());
		_incomingStream = new DataInputStream(_socket.getInputStream());
		handshake();
	}

	public void start() throws IOException {
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
			LPublicKey publicKey = (new BSONConvert<LPublicKey>()).deserialize(publicKeyHandshake.getPackagedData(),
					LPublicKey.class);
			LCipher.setRemotePublicKey(publicKey);

			{
				byte[] bsonPublicKey = (new BSONConvert<LPublicKey>()).serialize(LCipher.getLocalPublicKey());
				EnumSet<LMetadata> metadata = EnumSet.noneOf(LMetadata.class);
				metadata.add(LMetadata.FCLIENT);
				metadata.add(LMetadata.TROOT);
				LPacket packet = new LPacket(bsonPublicKey, metadata);
				sendPacket(packet);
			}

			LPacket aesHandshake = waitForResponse();
			byte[] aesHandshakePackage = LCipher.localDecrypt(aesHandshake.getPackagedData());
			LAESInfo remoteAESInfo = (new BSONConvert<LAESInfo>()).deserialize(aesHandshakePackage, LAESInfo.class);
			LCipher.loadRemoteAESInfo(remoteAESInfo);

			{
				byte[] aesData = (new BSONConvert<LAESInfo>()).serialize(LCipher.getLocalAESInfo());
				EnumSet<LMetadata> metadata = EnumSet.noneOf(LMetadata.class);
				metadata.add(LMetadata.FCLIENT);
				metadata.add(LMetadata.TROOT);
				metadata.add(LMetadata.ENCRYPTED);
				byte[] encrypted = LCipher.remoteEncrypt(aesData);
				LPacket packet = new LPacket(encrypted, metadata);
				sendPacket(packet);
			}

			String identifier = Base64.getEncoder().encodeToString(LocalConfig.loadConfig().getCIdentifier());
			CThumbprint thumbprint = new CThumbprint(identifier, Program.APPLICATION_VERSION, "local", Program.AUTH,
					SystemInfo.getHostname());

			{
				byte[] bsonThumbprint = (new BSONConvert<CThumbprint>()).serialize(thumbprint);
				EnumSet<LMetadata> metadata = EnumSet.noneOf(LMetadata.class);
				metadata.add(LMetadata.FCLIENT);
				metadata.add(LMetadata.TROOT);
				metadata.add(LMetadata.ENCRYPTED);
				byte[] encrypted = LCipher.localAESEncrypt(bsonThumbprint);
				LPacket packet = new LPacket(encrypted, metadata);
				sendPacket(packet);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	private void keepListening() throws IOException {
		while (true) {
			LPacket packet = waitForResponse();
			if (packet != null) {
				System.out.println("CM : " + LMetadata.fromByte(packet.getMetadata()));
			} else {
				throw new IOException("Malformed packet!");
			}
		}
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
