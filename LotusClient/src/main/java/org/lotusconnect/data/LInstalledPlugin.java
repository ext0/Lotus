package org.lotusconnect.data;

public class LInstalledPlugin {
	private String _name;
	private String _description;
	private String _author;
	private int _version;
	private String _classPathNameData;
	private byte[] _classData;

	private boolean _enabled;

	public LInstalledPlugin() {
		
	}
	
	public LInstalledPlugin(String name, String description, String author, int version, String classPathNameData,
			byte[] classData, boolean enabled) {
		_name = name;
		_description = description;
		_author = author;
		_version = version;
		_classPathNameData = classPathNameData;
		_classData = classData;
		_enabled = enabled;
	}

	public String getName() {
		return _name;
	}

	public void setName(String name) {
		_name = name;
	}

	public int getVersion() {
		return _version;
	}

	public void setVersion(int version) {
		_version = version;
	}

	public boolean getEnabled() {
		return _enabled;
	}

	public void setEnabled(boolean enabled) {
		_enabled = enabled;
	}

	public String getDescription() {
		return _description;
	}

	public void setDescription(String description) {
		_description = description;
	}

	public String getAuthor() {
		return _author;
	}

	public void setAuthor(String author) {
		_author = author;
	}

	public String getClassPathNameData() {
		return _classPathNameData;
	}

	public void setClassPathNameData(String classPathNameData) {
		_classPathNameData = classPathNameData;
	}

	public byte[] getClassData() {
		return _classData;
	}

	public void setClassData(byte[] classData) {
		_classData = classData;
	}
}
