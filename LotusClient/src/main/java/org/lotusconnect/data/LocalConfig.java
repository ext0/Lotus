package org.lotusconnect.data;

import java.io.File;
import java.io.IOException;
import java.util.UUID;

import org.apache.commons.io.FileUtils;
import org.lotusconnect.main.Program;

public class LocalConfig {
	private byte[] _cidentifier;

	public static final File CONFIG_PATH = new File(".config");

	private static LocalConfig _config = null;

	public static LocalConfig loadConfig() {
		if (_config != null) {
			return _config;
		}
		LocalConfig config;
		try {
			byte[] buffer = FileUtils.readFileToByteArray(CONFIG_PATH);
			config = (new BSONConvert<LocalConfig>()).deserialize(buffer, LocalConfig.class);
		} catch (Exception e) {
			config = new LocalConfig();
		}
		_config = config;
		return config;
	}

	public static void generateConfig() {
		if (CONFIG_PATH.exists()) {
			loadConfig();
			return;
		}
		try {
			CONFIG_PATH.createNewFile();
			LocalConfig config = new LocalConfig();
			config.setCIdentifier(UUID.randomUUID().toString().getBytes());
			byte[] bson = (new BSONConvert<LocalConfig>()).serialize(config);
			FileUtils.writeByteArrayToFile(CONFIG_PATH, bson);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	private LocalConfig() {

	}

	public byte[] getCIdentifier() {
		return _cidentifier;
	}

	public void setCIdentifier(byte[] cIdentifier) {
		this._cidentifier = cIdentifier;
	}
}
