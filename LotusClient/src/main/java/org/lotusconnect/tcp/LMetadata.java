package org.lotusconnect.tcp;

import java.util.ArrayList;
import java.util.EnumSet;
import java.util.List;

public enum LMetadata {
	ENCRYPTED, HEARTBEAT, FCLIENT, FROOT, FWEB, TCLIENT, TROOT, TWEB;

	public static final EnumSet<LMetadata> ALL_OPTS = EnumSet.allOf(LMetadata.class);

	public static EnumSet<LMetadata> createSet(List<LMetadata> flags) {
		return EnumSet.copyOf(flags);
	}

	public static byte getByte(EnumSet<LMetadata> set) {
		int b = 0x00;
		LMetadata[] flags = LMetadata.values();
		for (int i = 0; i < flags.length; i++) {
			if (set.contains(flags[i])) {
				b |= (1 << i);
			}
		}
		return (byte) b;
	}

	public static EnumSet<LMetadata> fromByte(byte b) {
		int c = b;
		EnumSet<LMetadata> metadata = EnumSet.noneOf(LMetadata.class);
		LMetadata[] flags = LMetadata.values();
		for (int i = 0; i < flags.length; i++) {
			if ((c & (1 << i)) == (1 << i)) {
				metadata.add(flags[i]);
			}
		}
		return metadata;
	}
}
