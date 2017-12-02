package org.lotusconnect.plugin;

import org.apache.log4j.Logger;

public class Log4JPluginLogger implements PluginLogger {

	private static final Logger LOGGER = Logger.getLogger(Log4JPluginLogger.class);
	private static final String SFORMAT = "[%s] %s\n";
	private String _pluginName;

	public Log4JPluginLogger(String pluginName) {
		_pluginName = pluginName;
	}

	public void info(String message) {
		LOGGER.info(String.format(SFORMAT, _pluginName, message));
	}

	public void trace(String message) {
		LOGGER.trace(String.format(SFORMAT, _pluginName, message));
	}

	public void debug(String message) {
		LOGGER.debug(String.format(SFORMAT, _pluginName, message));
	}

	public void warn(String message) {
		LOGGER.warn(String.format(SFORMAT, _pluginName, message));
	}

	public void error(String message) {
		LOGGER.error(String.format(SFORMAT, _pluginName, message));
	}

}
