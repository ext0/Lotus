package org.lotusconnect.plugin;

import java.util.EnumSet;

import org.lotusconnect.data.LRequest;
import org.lotusconnect.data.LResponse;
import org.lotusconnect.tcp.LMetadata;
import org.lotusconnect.tcp.RConnection;

public class RResponder implements Responder {

	private RConnection _connection;
	private LRequest _request;

	public RResponder(RConnection connection, LRequest request) {
		_connection = connection;
		_request = request;

	}

	public void respondString(String string) throws Exception {
		LResponse response = new LResponse(_request, string);
		_connection.sendResponse(response, EnumSet.noneOf(LMetadata.class));
	}
}
