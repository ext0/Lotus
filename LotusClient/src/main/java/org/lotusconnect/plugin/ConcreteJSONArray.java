package org.lotusconnect.plugin;

import org.apache.log4j.Logger;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

public class ConcreteJSONArray implements JSONArray {

	private static Logger LOGGER = Logger.getLogger(ConcreteJSONArray.class);

	private ObjectMapper _mapper;
	private ArrayNode _node;

	public ConcreteJSONArray() {
		_mapper = new ObjectMapper();
		_node = _mapper.createArrayNode();
	}

	public void add(JSONObject obj) {
		_node.add((ObjectNode) obj.getObjectNode());
	}

	public String getJSON() {
		try {
			return _mapper.writeValueAsString(_node);
		} catch (JsonProcessingException e) {
			LOGGER.error("Failed to serialize object to JSON: " + e.getMessage());
			return null;
		}
	}
}
