package org.lotusconnect.data;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;

import com.fasterxml.jackson.annotation.JsonAutoDetect.Visibility;
import com.fasterxml.jackson.databind.MapperFeature;
import com.fasterxml.jackson.databind.ObjectMapper;

import de.undercouch.bson4jackson.BsonFactory;

public class BSONConvert<T> {

	private ObjectMapper mapper = new ObjectMapper(new BsonFactory());

	public BSONConvert() {
		mapper.configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true);  
		mapper.setVisibility(mapper.getSerializationConfig().getDefaultVisibilityChecker()
				.withFieldVisibility(Visibility.NONE).withGetterVisibility(Visibility.PUBLIC_ONLY)
				.withSetterVisibility(Visibility.PUBLIC_ONLY).withCreatorVisibility(Visibility.NONE));
	}

	public T deserialize(byte[] bson, Class<T> clazz) {
		ByteArrayInputStream memoryStream = new ByteArrayInputStream(bson);
		try {
			return mapper.readValue(memoryStream, clazz);
		} catch (Exception e) {
			e.printStackTrace();
			return null;
		}
	}

	public byte[] serialize(T obj) {
		ByteArrayOutputStream memoryStream = new ByteArrayOutputStream();
		try {
			mapper.writeValue(memoryStream, obj);
			return memoryStream.toByteArray();
		} catch (Exception e) {
			e.printStackTrace();
			return null;
		}
	}
}
