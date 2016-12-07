package org.lotusconnect.data;

public class LPublicKey {
	private String _modulus;
	private String _exponent;

	private LPublicKey() {

	}

	public LPublicKey(String modulus, String exponent) {
		_modulus = modulus;
		_exponent = exponent;
	}

	public String getModulus() {
		return _modulus;
	}

	public void setModulus(String modulus) {
		_modulus = modulus;
	}

	public String getExponent() {
		return _exponent;
	}

	public void setExponent(String exponent) {
		_exponent = exponent;
	}
}
