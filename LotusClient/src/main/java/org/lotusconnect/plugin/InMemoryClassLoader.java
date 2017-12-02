package org.lotusconnect.plugin;

public class InMemoryClassLoader extends ClassLoader {
	private byte[] _bytes;

	public InMemoryClassLoader(byte[] bytes) {
		_bytes = bytes;
	}

	public void defineClass(String fullClassName) {
		defineClass(fullClassName, _bytes, 0, _bytes.length);
	}
}
