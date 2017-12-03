package org.lotusconnect.plugin;

import java.io.ByteArrayOutputStream;
import java.util.Date;

import org.apache.log4j.Logger;
import org.lotusconnect.data.JSONConvert;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;

public class ConcreteJSONObject implements JSONObject {

	private static Logger LOGGER = Logger.getLogger(ConcreteJSONObject.class);

	private ObjectMapper _mapper;
	private ObjectNode _node;

	public ConcreteJSONObject() {
		_mapper = new ObjectMapper();
		_node = _mapper.createObjectNode();
	}

	public void setProperty(String name, String val) {
		_node.put(name, val);
	}

	public void setProperty(String name, int val) {
		_node.put(name, val);
	}

	public void setProperty(String name, double val) {
		_node.put(name, val);
	}

	public void setProperty(String name, long val) {
		_node.put(name, val);
	}

	public void setProperty(String name, boolean val) {
		_node.put(name, val);
	}

	public void setProperty(String name, float val) {
		_node.put(name, val);
	}

	public boolean hasProperty(String name) {
		return _node.has(name);
	}

	public String getJSON() {
		try {
			return _mapper.writeValueAsString(_node);
		} catch (JsonProcessingException e) {
			LOGGER.error("Failed to serialize object to JSON: " + e.getMessage());
			return null;
		}
	}

	public Object getObjectNode() {
		return _node;
	}
}
