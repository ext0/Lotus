package org.lotusconnect.plugin.filemanager;

import java.io.File;
import java.nio.file.Files;
import java.util.Base64;

import javax.swing.filechooser.FileSystemView;

import org.lotusconnect.plugin.JSONArray;
import org.lotusconnect.plugin.JSONObject;
import org.lotusconnect.plugin.JSONObjectFactory;
import org.lotusconnect.plugin.PluginLogger;
import org.lotusconnect.plugin.RequestHandler;
import org.lotusconnect.plugin.Responder;

public class FileManagerRequestHandler implements RequestHandler {

	private PluginLogger _logger;
	private JSONObjectFactory _factory;

	private JSONObject getNetworkFileObject(File file) {
		JSONObject object = _factory.getNewObjectInstance();
		object.setProperty("path", file.getAbsolutePath());
		object.setProperty("isFile", file.isFile());
		object.setProperty("name", file.getName());
		object.setProperty("lastModified", file.lastModified());

		boolean isFile = file.isFile();
		long size = -1;
		long used = -1;
		long capacity = -1;
		if (isFile) {
			size = file.length();
		} else {
			used = file.getTotalSpace() - file.getFreeSpace();
			capacity = file.getTotalSpace();
		}

		object.setProperty("size", size);
		object.setProperty("used", used);
		object.setProperty("capacity", capacity);

		return object;
	}

	public void onLoad(PluginLogger logger, JSONObjectFactory jsonObjectFactory) {
		_logger = logger;
		_logger.info("loaded file manager request handler!");
		_factory = jsonObjectFactory;
	}

	public boolean handleRequest(String command, String[] data, String id, Responder responder) throws Exception {
		if (command.equals("FILEMANAGERGETDRIVES")) {
			File[] roots = FileSystemView.getFileSystemView().getRoots();
			JSONArray array = _factory.getNewArrayInstance();
			for (int i = 0; i < roots.length; i++) {
				array.add(getNetworkFileObject(roots[i]));
			}
			String json = array.getJSON();
			responder.respondString(json);
			return true;
		} else if (command.equals("FILEMANAGERGETDIRECTORY")) {
			String path = data[1];
			File file = new File(path);
			File[] children = file.listFiles();
			JSONArray array = _factory.getNewArrayInstance();
			for (int i = 0; i < children.length; i++) {
				array.add(getNetworkFileObject(children[i]));
			}
			String json = array.getJSON();
			responder.respondString(json);
			return true;
		} else if (command.equals("FILEMANAGERGETPARENT")) {
			String path = data[1];
			File file = new File(path);
			File parent = file.getParentFile();
			String response;
			if (parent != null) {
				JSONObject parentObject = getNetworkFileObject(parent);
				response = parentObject.getJSON();
			} else {
				response = "FAIL";
			}
			responder.respondString(response);
			return true;
		} else if (command.equals("FILEMANAGERDOWNLOADFILE")) {
			String path = data[1];
			try {
				String encoded = Base64.getEncoder().encodeToString(Files.readAllBytes(new File(path).toPath()));
				responder.respondString(encoded);
			} catch (Exception e) {
				_logger.error("Failed to read file " + path + " : " + e.getMessage());
			}
			return true;
		} else if (command.equals("FILEMANAGERDELETEFILE")) {
			String path = data[1];
			try {
				File file = new File(path);
				boolean success = file.delete();
				responder.respondString((success) ? "SUCCESS" : "FAIL");
			} catch (Exception e) {
				_logger.error("Failed to delete file " + path + " : " + e.getMessage());
				responder.respondString("FAIL");
			}
			return true;
		} else if (command.equals("FILEMANAGEREXECUTEFILE")) {
			String path = data[1];
			try {
				Runtime.getRuntime().exec(path);
				responder.respondString("SUCCESS");
			} catch (Exception e) {
				_logger.error("Failed to execute file " + path + " : " + e.getMessage());
				responder.respondString("FAIL");
			}
			return true;
		}
		return false;
	}

}
