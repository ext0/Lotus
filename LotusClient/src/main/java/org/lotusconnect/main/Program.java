package org.lotusconnect.main;

import java.io.IOException;

import org.apache.log4j.Logger;
import org.lotusconnect.data.LocalConfig;
import org.lotusconnect.tcp.LCipher;
import org.lotusconnect.tcp.RConnection;

public class Program {

	public static final int APPLICATION_VERSION = 2;
	public static final String AUTH = "3b82fa6f6d6dacc813d98287433586cd";

	private static final Logger LOGGER = Logger.getLogger(Program.class);

	public static void main(String[] args) {
		LOGGER.info("Starting Lotus Client...");
		LocalConfig.generateConfig();
		LOGGER.info("Configuration information successfully loaded.");

		LOGGER.debug("Generating cryptographic keys...");
		boolean symmetricSuccess = LCipher.generateSymmetricCipher();
		if (!symmetricSuccess) {
			errorExit();
		}
		boolean assymmetricSuccess = LCipher.generateAssymmetricCipher();
		if (!assymmetricSuccess) {
			errorExit();
		}

		LOGGER.info("Attempting to open connection...");
		RConnection connection = new RConnection("192.168.2.8", 8888);
		while (!attemptConnection(connection)) {
			try {
				Thread.sleep(1000);
			} catch (InterruptedException e) {
				LOGGER.error("Error occurred waiting for initial connection retry: " + e.getMessage());
				errorExit();
			}
		}
		LOGGER.info("Successfully connected to remote root!");
		try {
			connection.start();
		} catch (Exception e) {
			LOGGER.fatal("Error occured while communicating with root: " + e.getMessage());
			e.printStackTrace();
			errorExit();
		}
	}

	public static void errorExit() {
		System.exit(1);
	}

	public static boolean attemptConnection(RConnection connection) {
		try {
			connection.connect();
			return true;
		} catch (Exception e) {
			LOGGER.error("Failed to connect to " + connection + ", retrying...");
			return false;
		}
	}

}
