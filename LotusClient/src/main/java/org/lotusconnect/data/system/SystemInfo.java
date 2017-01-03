package org.lotusconnect.data.system;

import java.io.IOException;
import java.net.InetAddress;
import java.net.UnknownHostException;

import org.apache.commons.lang3.SystemUtils;

public class SystemInfo {
	public static String getHostname() throws UnknownHostException {
		return InetAddress.getLocalHost().getHostName();
	}

	public static boolean shutdown() throws IOException {
		String shutdownCommand = null;
		if (SystemUtils.IS_OS_LINUX || SystemUtils.IS_OS_MAC || SystemUtils.IS_OS_MAC_OSX
				|| SystemUtils.IS_OS_UNIX)
			shutdownCommand = "shutdown -h now";
		else if (SystemUtils.IS_OS_WINDOWS)
			shutdownCommand = "shutdown -s";
		else
			return false;

		Runtime.getRuntime().exec(shutdownCommand);
		return true;
	}

	public static boolean logoff() throws IOException {
		String logoffCommand = null;

		if (SystemUtils.IS_OS_WINDOWS) {
			logoffCommand = "shutdown -l";
		} else if (SystemUtils.IS_OS_MAC || SystemUtils.IS_OS_MAC_OSX) {
			logoffCommand = "logout";
		} else {
			return false;
		}
		Runtime.getRuntime().exec(logoffCommand);
		return true;
	}
	
	public static boolean restart() throws IOException {
		String restartCommand = null, t = "now";

		if (SystemUtils.IS_OS_WINDOWS) {
			restartCommand = "shutdown -r";
		} else if (SystemUtils.IS_OS_MAC || SystemUtils.IS_OS_MAC_OSX) {
			restartCommand = "shutdown -r";
		} else {
			return false;
		}
		Runtime.getRuntime().exec(restartCommand);
		return true;
	}
}
