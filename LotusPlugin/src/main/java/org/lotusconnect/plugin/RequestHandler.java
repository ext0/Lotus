package org.lotusconnect.plugin;

public interface RequestHandler {
	void handleRequest(String command, String data, String id, Responder responder);
}
