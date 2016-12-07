package org.lotusconnect.tcp;

import java.io.IOException;
import java.math.BigInteger;
import java.nio.ByteBuffer;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.KeyFactory;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.SecureRandom;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.RSAPrivateKeySpec;
import java.security.spec.RSAPublicKeySpec;
import java.util.Arrays;
import java.util.Base64;

import javax.crypto.BadPaddingException;
import javax.crypto.Cipher;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.KeyGenerator;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

import org.apache.commons.io.output.ByteArrayOutputStream;
import org.lotusconnect.data.LAESInfo;
import org.lotusconnect.data.LPublicKey;

public class LCipher {

	public static final String ALGORITHM = "RSA";

	private static LPublicKey _remotePublicKey;
	private static LPublicKey _localPublicKey;

	private static final int RSA_KEY_SIZE = 4096;
	private static final int RSA_BUFFER_SIZE = 512;
	private static final int AES_KEY_SIZE = 16;

	private static Cipher _localAsymmetricCipher;
	private static Cipher _remoteAsymmetricCipher;
	private static Cipher _localSymmetricCipher;
	private static Cipher _remoteSymmetricCipher;

	private static LAESInfo _localAESInfo;

	public static void generateSymmetricCipher() throws NoSuchAlgorithmException, NoSuchPaddingException,
			InvalidKeyException, InvalidAlgorithmParameterException {
		SecureRandom random = new SecureRandom();
		byte[] key = new byte[AES_KEY_SIZE];
		random.nextBytes(key);

		SecretKeySpec keySpec = new SecretKeySpec(key, "AES");

		_localSymmetricCipher = Cipher.getInstance("AES/CBC/PKCS5PADDING");
		_localSymmetricCipher.init(Cipher.ENCRYPT_MODE, keySpec);

		_localAESInfo = new LAESInfo(Base64.getEncoder().encodeToString(_localSymmetricCipher.getIV()),
				Base64.getEncoder().encodeToString(key));

	}

	public static void generateAssymmetricCipher()
			throws NoSuchAlgorithmException, InvalidKeySpecException, InvalidKeyException, NoSuchPaddingException {
		KeyPairGenerator keyPairGenerator = KeyPairGenerator.getInstance("RSA");
		keyPairGenerator.initialize(RSA_KEY_SIZE);
		KeyPair keyPair = keyPairGenerator.generateKeyPair();
		PublicKey publicKey = keyPair.getPublic();
		KeyFactory keyFactory = KeyFactory.getInstance("RSA");
		RSAPublicKeySpec rsaPubKeySpec = keyFactory.getKeySpec(publicKey, RSAPublicKeySpec.class);
		byte[] modulus = rsaPubKeySpec.getModulus().toByteArray();
		byte[] exponent = rsaPubKeySpec.getPublicExponent().toByteArray();
		_localPublicKey = new LPublicKey(Base64.getEncoder().encodeToString(Arrays.copyOfRange(modulus, 1, modulus.length)),
				Base64.getEncoder().encodeToString(Arrays.copyOfRange(exponent, 0, exponent.length)));
		RSAPrivateKeySpec rsaPrivKeySpec = keyFactory.getKeySpec(keyPair.getPrivate(), RSAPrivateKeySpec.class);
		_localAsymmetricCipher = Cipher.getInstance("RSA/ECB/PKCS1Padding");
		_localAsymmetricCipher.init(Cipher.DECRYPT_MODE, keyPair.getPrivate());
	}

	public static void loadRemoteAESInfo(LAESInfo info) throws InvalidKeyException, InvalidAlgorithmParameterException,
			NoSuchAlgorithmException, NoSuchPaddingException {
		IvParameterSpec ivSpec = new IvParameterSpec(Base64.getDecoder().decode(info.getIV()));
		SecretKeySpec skeySpec = new SecretKeySpec(Base64.getDecoder().decode(info.getKey()), "AES");
		_remoteSymmetricCipher = Cipher.getInstance("AES/CBC/PKCS5PADDING");
		_remoteSymmetricCipher.init(Cipher.DECRYPT_MODE, skeySpec, ivSpec);
	}

	public static byte[] localAESEncrypt(byte[] data) throws IllegalBlockSizeException, BadPaddingException {

		byte[] cipherData = _localSymmetricCipher.doFinal(data);
		return cipherData;
	}

	public static byte[] remoteAESDecrypt(byte[] data) throws IllegalBlockSizeException, BadPaddingException {
		byte[] cipherData = _remoteSymmetricCipher.doFinal(data);
		return cipherData;
	}

	public static byte[] remoteEncrypt(byte[] data) throws IllegalBlockSizeException, BadPaddingException, IOException {
		byte[] block;
		ByteArrayOutputStream output = new ByteArrayOutputStream();
		int read = 0;
		while (read < data.length) {
			block = new byte[RSA_BUFFER_SIZE - 11];
			int toRead = Math.min(data.length - read, block.length);
			System.arraycopy(data, read, block, 0, toRead);
			read += toRead;
			output.write(_remoteAsymmetricCipher.doFinal(block));
		}
		byte[] ret = output.toByteArray();
		output.close();
		return ret;
	}

	public static byte[] localDecrypt(byte[] data) throws IllegalBlockSizeException, BadPaddingException, IOException {
		byte[] block;
		ByteArrayOutputStream output = new ByteArrayOutputStream();
		int read = 0;
		while (read < data.length) {
			block = new byte[RSA_BUFFER_SIZE];
			int toRead = Math.min(data.length - read, block.length);
			System.arraycopy(data, read, block, 0, toRead);
			read += toRead;
			output.write(_localAsymmetricCipher.doFinal(block));
		}
		byte[] ret = output.toByteArray();
		output.close();
		return ret;
	}

	public static LPublicKey getLocalPublicKey() {
		return _localPublicKey;
	}

	public static void setRemotePublicKey(LPublicKey key)
			throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, InvalidKeySpecException {
		_remotePublicKey = key;
		byte[] modulusBytes = Base64.getDecoder().decode(_remotePublicKey.getModulus());
		byte[] exponentBytes = Base64.getDecoder().decode(_remotePublicKey.getExponent());
		BigInteger modulus = new BigInteger(1, modulusBytes);
		BigInteger publicExponent = new BigInteger(1, exponentBytes);

		RSAPublicKeySpec rsaPubKey = new RSAPublicKeySpec(modulus, publicExponent);
		KeyFactory fact = KeyFactory.getInstance("RSA");
		PublicKey pubKey = fact.generatePublic(rsaPubKey);
		_remoteAsymmetricCipher = Cipher.getInstance("RSA/ECB/PKCS1Padding");
		_remoteAsymmetricCipher.init(Cipher.ENCRYPT_MODE, pubKey);
	}

	public static LAESInfo getLocalAESInfo() {
		return _localAESInfo;
	}
}
