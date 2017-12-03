package org.lotusconnect.data;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.util.Base64;

import org.apache.log4j.Logger;

import com.fasterxml.jackson.annotation.JsonAutoDetect.Visibility;
import com.fasterxml.jackson.core.JsonFactory;
import com.fasterxml.jackson.databind.MapperFeature;
import com.fasterxml.jackson.databind.ObjectMapper;

public class JSONConvert {

	private ObjectMapper mapper = new ObjectMapper(new JsonFactory());

	private static Logger LOGGER = Logger.getLogger(JSONConvert.class);

	public JSONConvert() {
		mapper.configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true);
		mapper.setVisibility(mapper.getSerializationConfig().getDefaultVisibilityChecker()
				.withFieldVisibility(Visibility.NONE).withGetterVisibility(Visibility.PUBLIC_ONLY)
				.withSetterVisibility(Visibility.PUBLIC_ONLY).withCreatorVisibility(Visibility.NONE));
	}

	public Object fromBytes(byte[] bson, Class<Object> clazz) {
		ByteArrayInputStream memoryStream = new ByteArrayInputStream(bson);
		try {
			return mapper.readValue(memoryStream, clazz);
		} catch (Exception e) {
			LOGGER.error("Failed to deserialize object from JSON: " + e.getMessage());
			return null;
		}
	}

	public byte[] toBytes(Object obj) {
		ByteArrayOutputStream memoryStream = new ByteArrayOutputStream();
		try {
			mapper.writeValue(memoryStream, obj);
			return memoryStream.toByteArray();
		} catch (Exception e) {
			LOGGER.error("Failed to serialize object to JSON: " + e.getMessage());
			return null;
		}
	}
	
	public String toBase64(Object obj) {
		return Base64.getEncoder().encodeToString(toBytes(obj));
	}
	
	public Object fromBase64(String base64, Class<Object> clazz) {
		return fromBytes(Base64.getDecoder().decode(base64), clazz);
	}
}