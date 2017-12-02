package org.lotusconnect.plugin.filemanager;

import org.lotusconnect.plugin.PluginLogger;
import org.lotusconnect.plugin.RequestHandler;
import org.lotusconnect.plugin.Responder;

public class FileManagerRequestHandler implements RequestHandler {

	private PluginLogger _logger;

	public void onLoad(PluginLogger logger) {
		_logger = logger;
		_logger.info("loaded file manager request handler!");
	}

	public boolean handleRequest(String command, String[] data, String id, Responder responder) {
		_logger.info("handle request: " + command + " " + id);
		return false;
	}

}
