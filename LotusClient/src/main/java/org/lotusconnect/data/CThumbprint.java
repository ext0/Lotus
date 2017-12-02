package org.lotusconnect.data;

public class CThumbprint {
	private String _cidentifier;
	private int _cversion;
	private String _cip;
	private String _auth;
	private String _hostname;
	private LInstalledPlugin[] _installedPlugins;

	public CThumbprint(String cIdentifier, int cVersion, String cIP, String auth, String hostname,
			LInstalledPlugin[] installedPlugins) {
		_cidentifier = cIdentifier;
		_cversion = cVersion;
		_cip = cIP;
		_auth = auth;
		_hostname = hostname;
		_installedPlugins = installedPlugins;
	}

	public String getCIdentifier() {
		return _cidentifier;
	}

	public void setCIdentifier(String cIdentifier) {
		_cidentifier = cIdentifier;
	}

	public int getCVersion() {
		return _cversion;
	}

	public void setCVersion(int cVersion) {
		_cversion = cVersion;
	}

	public String getCIP() {
		return _cip;
	}

	public void setCIP(String cIP) {
		_cip = cIP;
	}

	public String getAuth() {
		return _auth;
	}

	public void setAuth(String auth) {
		_auth = auth;
	}

	public String getHostname() {
		return _hostname;
	}

	public void setHostname(String hostname) {
		_hostname = hostname;
	}

	public LInstalledPlugin[] getInstalledPlugins() {
		return _installedPlugins;
	}

	public void setInstalledPlugins(LInstalledPlugin[] installedPlugins) {
		_installedPlugins = installedPlugins;
	}

}
