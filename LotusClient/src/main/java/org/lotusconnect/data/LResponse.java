package org.lotusconnect.data;

public class LResponse {
	private String _response;
	private String _data;
	private String _id;

	private LResponse() {
		
	}
	
	public LResponse(String response, String data, String id) {
		_response = response;
		_data = data;
		_id = id;
	}

	public LResponse(LRequest request, String data) {
		_response = request.getCommand();
		_data = data;
		_id = request.getID();
	}

	public String getResponse() {
		return _response;
	}

	public void setResponse(String response) {
		this._response = response;
	}

	public String getData() {
		return _data;
	}

	public void setData(String data) {
		this._data = data;
	}

	public String getID() {
		return _id;
	}

	public void setID(String id) {
		this._id = id;
	}

}
