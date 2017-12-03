package org.lotusconnect.plugin;

public class ConcreteJSONObjectFactory implements JSONObjectFactory {

	public JSONObject getNewObjectInstance() {
		return new ConcreteJSONObject();
	}

	public JSONArray getNewArrayInstance() {
		return new ConcreteJSONArray();
	}

}
