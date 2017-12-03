package org.lotusconnect.plugin;

public interface RequestHandler {
	void onLoad(PluginLogger logger, JSONObjectFactory jsonObjectFactory);

	boolean handleRequest(String command, String[] data, String id, Responder responder) throws Exception;
}
