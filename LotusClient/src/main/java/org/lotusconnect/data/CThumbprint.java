package org.lotusconnect.data;

public class CThumbprint {
	private String _cidentifier;
	private int _cversion;
	private String _cip;
	private String _auth;
	private String _hostname;

	public CThumbprint(String cIdentifier, int cVersion, String cIP, String auth, String hostname) {
		_cidentifier = cIdentifier;
		_cversion = cVersion;
		_cip = cIP;
		_auth = auth;
		_hostname = hostname;
	}

	public String getCIdentifier() {
		return _cidentifier;
	}

	public void setCIdentifier(String cIdentifier) {
		this._cidentifier = cIdentifier;
	}

	public int getCVersion() {
		return _cversion;
	}

	public void setCVersion(int cVersion) {
		this._cversion = cVersion;
	}

	public String getCIP() {
		return _cip;
	}

	public void setCIP(String cIP) {
		this._cip = cIP;
	}

	public String getAuth() {
		return _auth;
	}

	public void setAuth(String auth) {
		this._auth = auth;
	}

	public String getHostname() {
		return _hostname;
	}

	public void setHostname(String hostname) {
		_hostname = hostname;
	}

}
