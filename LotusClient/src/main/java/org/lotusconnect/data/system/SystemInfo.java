package org.lotusconnect.data.system;

import java.net.InetAddress;
import java.net.UnknownHostException;

public class SystemInfo {
	public static String getHostname() throws UnknownHostException {
		return InetAddress.getLocalHost().getHostName();
	}
}
