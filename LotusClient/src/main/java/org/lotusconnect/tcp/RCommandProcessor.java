package org.lotusconnect.tcp;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.EnumSet;
import java.util.List;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.swing.filechooser.FileSystemView;

import org.apache.log4j.Logger;
import org.lotusconnect.data.JSONConvert;
import org.lotusconnect.data.LRequest;
import org.lotusconnect.data.LResponse;

public class RCommandProcessor {
	
	private static Logger LOGGER = Logger.getLogger(RCommandProcessor.class);

	private RConnection _connection;
	
	public RCommandProcessor(RConnection connection) {
		_connection = connection;
	}
	
	public void processRequest(LRequest request) throws IllegalBlockSizeException, BadPaddingException, IOException {
		if (request.getCommand().equals("CGETDRIVES")) {
			File[] paths = File.listRoots();
			String[] stringPaths = new String[paths.length];
			for (int i = 0; i < paths.length; i++) {
				stringPaths[i] = paths[i].getAbsolutePath();
			}
			String base64 = (new JSONConvert<String[]>()).toBase64(stringPaths);
			LResponse response = new LResponse(request, base64);
			_connection.sendResponse(response, EnumSet.noneOf(LMetadata.class));
		}
	}

	public void processResponse(LResponse response) {

	}
}