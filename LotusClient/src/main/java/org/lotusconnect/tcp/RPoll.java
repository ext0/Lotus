package org.lotusconnect.tcp;

import java.io.IOException;
import java.util.EnumSet;
import java.util.TimerTask;

public class RPoll extends TimerTask {

	private RConnection _connection;
	
	public RPoll(RConnection connection) {
		_connection = connection;
	}

	@Override
	public void run() {
		try {
			_connection.sendPacket(getHeartbeatPacket());
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	private LPacket getHeartbeatPacket() {
		EnumSet<LMetadata> metadata = EnumSet.noneOf(LMetadata.class);
		metadata.add(LMetadata.HEARTBEAT);
	    return new LPacket(new byte[] { }, metadata);
	}

}
