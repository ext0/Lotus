package org.lotusconnect.data;

import java.util.UUID;

public class LRequest {

	private String _auth;
	private String _command;
	private String[] _parameters;
	private boolean _asyncCallback;
	private String _id;

	private LRequest() {

	}

	public LRequest(String auth, String command, String[] parameters, boolean asyncCallback, String id) {
		_auth = auth;
		_command = command;
		_parameters = parameters;
		_asyncCallback = asyncCallback;
		_id = id;
	}

	public String getAuth() {
		return _auth;
	}

	public void setAuth(String auth) {
		_auth = auth;
	}

	public String getCommand() {
		return _command;
	}

	public void setCommand(String command) {
		_command = command;
	}

	public String[] getParameters() {
		return _parameters;
	}

	public void setParameters(String[] parameters) {
		_parameters = parameters;
	}

	public boolean isAsyncCallback() {
		return _asyncCallback;
	}

	public void setASyncCallback(boolean asyncCallback) {
		_asyncCallback = asyncCallback;
	}

	public String getID() {
		return _id;
	}

	public void setID(String id) {
		_id = id;
	}
}
