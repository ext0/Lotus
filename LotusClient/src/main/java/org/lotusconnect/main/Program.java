package org.lotusconnect.main;

import java.io.File;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.spec.InvalidKeySpecException;
import java.util.ArrayList;
import java.util.EnumSet;

import javax.crypto.NoSuchPaddingException;

import org.lotusconnect.data.BSONConvert;
import org.lotusconnect.data.LocalConfig;
import org.lotusconnect.tcp.LCipher;
import org.lotusconnect.tcp.LMetadata;
import org.lotusconnect.tcp.RConnection;

public class Program {

	public static final int APPLICATION_VERSION = 1;
	public static final String AUTH = "d615412bcc73ad661ca2ab73f69b4482";

	public static void main(String[] args) throws InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidAlgorithmParameterException, InvalidKeySpecException {
		LocalConfig.generateConfig();
		/*
		 * List<CMetadata> metadata = new ArrayList<CMetadata>();
		 * metadata.add(CMetadata.ENCRYPTED); metadata.add(CMetadata.FCLIENT);
		 * metadata.add(CMetadata.TROOT); EnumSet<CMetadata> set =
		 * CMetadata.createSet(metadata); byte b = CMetadata.getByte(set);
		 * EnumSet<CMetadata> unset = CMetadata.fromByte(b);
		 */
		LCipher.generateSymmetricCipher();
		LCipher.generateAssymmetricCipher();
		RConnection connection = new RConnection("192.168.1.5", 25321);
		connection.connect();
	}

}
