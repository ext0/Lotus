package org.lotusconnect.plugin;

public interface RequestHandler {
	void onLoad(PluginLogger logger);

	boolean handleRequest(String command, String[] data, String id, Responder responder);
}
