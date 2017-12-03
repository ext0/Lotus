package org.lotusconnect.plugin;

public interface JSONObject {
	public void setProperty(String name, String val);

	public void setProperty(String name, int val);

	public void setProperty(String name, double val);

	public void setProperty(String name, long val);

	public void setProperty(String name, boolean val);

	public void setProperty(String name, float val);
	
	public boolean hasProperty(String name);
	
	public Object getObjectNode();

	public String getJSON();
}
