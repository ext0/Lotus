package org.lotusconnect.data;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.util.Base64;

import org.apache.log4j.Logger;

import com.fasterxml.jackson.annotation.JsonAutoDetect.Visibility;
import com.fasterxml.jackson.databind.MapperFeature;
import com.fasterxml.jackson.databind.ObjectMapper;

import de.undercouch.bson4jackson.BsonFactory;

public class BSONConvert<T> {

	private ObjectMapper mapper = new ObjectMapper(new BsonFactory());

	private static Logger LOGGER = Logger.getLogger(BSONConvert.class);

	public BSONConvert() {
		mapper.configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true);
		mapper.setVisibility(mapper.getSerializationConfig().getDefaultVisibilityChecker()
				.withFieldVisibility(Visibility.NONE).withGetterVisibility(Visibility.PUBLIC_ONLY)
				.withSetterVisibility(Visibility.PUBLIC_ONLY).withCreatorVisibility(Visibility.NONE));
	}

	public T fromBytes(byte[] bson, Class<T> clazz) {
		ByteArrayInputStream memoryStream = new ByteArrayInputStream(bson);
		try {
			return mapper.readValue(memoryStream, clazz);
		} catch (Exception e) {
			LOGGER.error("Failed to deserialize object from BSON: " + e.getMessage());
			return null;
		}
	}

	public byte[] toBytes(T obj) {
		ByteArrayOutputStream memoryStream = new ByteArrayOutputStream();
		try {
			mapper.writeValue(memoryStream, obj);
			return memoryStream.toByteArray();
		} catch (Exception e) {
			LOGGER.error("Failed to serialize object to BSON: " + e.getMessage());
			return null;
		}
	}
	
	public String toBase64(T obj) {
		return Base64.getEncoder().encodeToString(toBytes(obj));
	}
	
	public T fromBase64(String base64, Class<T> clazz) {
		return fromBytes(Base64.getDecoder().decode(base64), clazz);
	}
}
