package org.lotusconnect.plugin;

public interface PluginLogger {
	public void info(String message);
	public void trace(String message);
	public void debug(String message);
	public void warn(String message);
	public void error(String message);
}
