package org.lotusconnect.data;

public class LAESInfo {
	private String _iv;
	private String _key;

	private LAESInfo() {

	}

	public LAESInfo(String iv, String key) {
		_iv = iv;
		_key = key;
	}

	public String getIV() {
		return _iv;
	}

	public void setIV(String iv) {
		_iv = iv;
	}

	public String getKey() {
		return _key;
	}

	public void setKey(String key) {
		_key = key;
	}
}
