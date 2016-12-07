package org.lotusconnect.tcp;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.EnumSet;
import java.util.zip.GZIPInputStream;
import java.util.zip.GZIPOutputStream;

public class LPacket {
	public static final int METADATA_LENGTH = 1;
	public static final int LENGTH_LENGTH = 4;
	public static final int DECOMPRESS_BUFFER_SIZE = 4096;

	private byte[] _data;
	private byte[] _packetBuffer;
	private byte _metadata;

	public LPacket(byte[] structured, EnumSet<LMetadata> metadata) {
		_data = structured;
		_metadata = LMetadata.getByte(metadata);
		byte[] data = null;
		try {
			ByteArrayOutputStream byteStream = new ByteArrayOutputStream(structured.length);
			try {
				GZIPOutputStream zipStream = new GZIPOutputStream(byteStream);
				try {
					zipStream.write(structured);
				} finally {
					zipStream.close();
				}
			} finally {
				byteStream.close();
			}
			data = byteStream.toByteArray();
		} catch (Exception e) {
			e.printStackTrace();
		}
		byte[] length = ByteBuffer.allocate(4).order(ByteOrder.LITTLE_ENDIAN).putInt(data.length).array();
		_packetBuffer = new byte[length.length + data.length + METADATA_LENGTH];
		System.arraycopy(length, 0, _packetBuffer, 0, length.length);
		_packetBuffer[length.length] = _metadata;
		System.arraycopy(data, 0, _packetBuffer, length.length + METADATA_LENGTH, data.length);
	}

	public LPacket(byte[] length, byte metadata, byte[] raw) {
		_metadata = metadata;
		try {
			ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
			if (raw.length > METADATA_LENGTH) {
				ByteArrayInputStream byteStream = new ByteArrayInputStream(raw);
				try {
					GZIPInputStream zipStream = new GZIPInputStream(byteStream);
					byte[] buffer = new byte[DECOMPRESS_BUFFER_SIZE];
					int read;
					while ((read = zipStream.read(buffer, 0, DECOMPRESS_BUFFER_SIZE)) != -1) {
						outputStream.write(buffer, 0, read);
					}
				} finally {
					byteStream.close();
					outputStream.close();
				}
				_data = outputStream.toByteArray();
			} else {
				_data = raw;
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public byte[] getPacket() {
		return _packetBuffer;
	}

	public int getPacketLength() {
		return _packetBuffer.length;
	}

	public byte[] getPackagedData() {
		return _data;
	}

	public int getPackagedDataLength() {
		return _data.length;
	}

	public byte getMetadata() {
		return _metadata;
	}
}
