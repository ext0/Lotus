package org.lotusconnect.plugin;

import org.lotusconnect.data.JSONConvert;
import org.lotusconnect.data.LRequest;

public class Plugin {
	private String _author;
	private int _version;
	private String _name;
	private String _description;

	private byte[] _compiledClassData;
	private String _absoluteClassName;
	private RequestHandler _compiled;
	private InMemoryClassLoader _classLoader;
	private Class<RequestHandler> _class;

	private Log4JPluginLogger _logger;

	public Plugin() {
	}

	public Plugin(String name, String description, int version, String author, String absoluteClassName,
			byte[] compiledClassData) {
		_name = name;
		_description = description;
		_version = version;
		_author = author;
		_absoluteClassName = absoluteClassName;
		_compiledClassData = compiledClassData;
	}

	public String getName() {
		return _name;
	}

	public void setName(String name) {
		_name = name;
	}

	public String getDescription() {
		return _description;
	}

	public void setDescription(String description) {
		_description = description;
	}

	public String getAuthor() {
		return _author;
	}

	public void setAuthor(String author) {
		_author = author;
	}

	public String getAbsoluteClassPathName() {
		return _absoluteClassName;
	}

	public void setAbsoluteClassPathName(String absoluteClassName) {
		_absoluteClassName = absoluteClassName;
	}

	public byte[] getClassData() {
		return _compiledClassData;
	}

	public void setClassData(byte[] classData) {
		_compiledClassData = classData;
	}

	public int getVersion() {
		return _version;
	}

	public void setVersion(int version) {
		_version = version;
	}

	@SuppressWarnings("unchecked")
	public void compile() throws InstantiationException, IllegalAccessException, ClassNotFoundException {
		_classLoader = new InMemoryClassLoader(_compiledClassData);

		_classLoader.defineClass(_absoluteClassName);
		_class = (Class<RequestHandler>) _classLoader.loadClass(_absoluteClassName);
		_compiled = (RequestHandler) _class.newInstance();
	}

	public boolean passRequest(LRequest request, RResponder responder) throws Exception {
		return _compiled.handleRequest(request.getCommand(), request.getParameters(), request.getID(), responder);
	}

	public void bootstrap() {
		_logger = new Log4JPluginLogger(_name);
		_compiled.onLoad(_logger, new ConcreteJSONObjectFactory());
	}

	public int hashCode() {
		return _name.hashCode();
	}

	public boolean equals(Object other) {
		if (!(other instanceof Plugin)) {
			return false;
		}
		Plugin plugin = (Plugin) other;
		return plugin.getName().equals(getName());
	}
}
