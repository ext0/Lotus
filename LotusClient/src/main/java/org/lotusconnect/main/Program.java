package org.lotusconnect.main;

import java.io.IOException;

import org.apache.log4j.Logger;
import org.lotusconnect.data.LocalConfig;
import org.lotusconnect.tcp.LCipher;
import org.lotusconnect.tcp.RConnection;

public class Program {

	public static final int APPLICATION_VERSION = 1;
	public static final String AUTH = "d615412bcc73ad661ca2ab73f69b4482";

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
		RConnection connection = new RConnection("192.168.1.5", 25321);
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
		} catch (IOException e) {
			LOGGER.fatal("Error occured while communicating with root: " + e.getMessage());
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
