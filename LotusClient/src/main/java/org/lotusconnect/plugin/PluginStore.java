package org.lotusconnect.plugin;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.Arrays;

import org.apache.log4j.Logger;
import org.lotusconnect.data.BSONConvert;
import org.lotusconnect.data.LInstalledPlugin;
import org.lotusconnect.main.Program;

public class PluginStore {

	private static final Logger LOGGER = Logger.getLogger(PluginStore.class);

	private static ArrayList<Plugin> _loadedPlugins = new ArrayList<Plugin>();

	public static LInstalledPlugin[] getInstalledPluginDefinitions() {
		LInstalledPlugin[] installedPlugins = new LInstalledPlugin[_loadedPlugins.size()];
		for (int i = 0; i < installedPlugins.length; i++) {
			Plugin plugin = _loadedPlugins.get(i);
			installedPlugins[i] = new LInstalledPlugin(plugin.getName(), plugin.getDescription(), plugin.getAuthor(),
					plugin.getVersion(), plugin.getAbsoluteClassPathName(), plugin.getClassData(), true);
		}
		return installedPlugins;
	}

	public static boolean savePluginsToDisk() {
		Plugin[] plugins = new Plugin[_loadedPlugins.size()];
		plugins = _loadedPlugins.toArray(plugins);
		byte[] data = new BSONConvert<Plugin[]>().toBytes(plugins);
		try {
			FileOutputStream stream = new FileOutputStream(Program.PLUGIN_STORE);
			stream.write(data);
			stream.close();
			LOGGER.debug("Saved " + plugins.length + " plugins to disk!");
			return true;
		} catch (IOException e) {
			LOGGER.error("Failed to save plugins! " + e.getMessage());
			return false;
		}
	}

	public static boolean loadPluginsFromDisk() {
		File pluginPath = new File(Program.PLUGIN_STORE);
		if (!pluginPath.exists()) {
			LOGGER.warn("Could not load plugins from disk! Plugin file " + Program.PLUGIN_STORE
					+ " not found. If this is the first time running this client, ignore this warning.");
			return false;
		}
		try {
			byte[] data = Files.readAllBytes(pluginPath.toPath());
			Plugin[] plugins = new BSONConvert<Plugin[]>().fromBytes(data, Plugin[].class);
			for (Plugin plugin : plugins) {
				loadPlugin(plugin);
			}
			LOGGER.debug("Loaded " + _loadedPlugins.size() + " plugins from disk!");
			return true;
		} catch (IOException e) {
			LOGGER.error("Failed to load plugins! " + e.getMessage());
			return false;
		}
	}

	private static boolean loadPlugin(Plugin build) {
		if (_loadedPlugins.contains(build)) {
			LOGGER.warn("Failed to activate " + build.getName() + ": already loaded.");
			return false;
		}
		try {
			LOGGER.debug("Compiling plugin: " + build.getName());
			build.compile();
			LOGGER.debug("Bootstrapping plugin: " + build.getName());
			build.bootstrap();
			_loadedPlugins.add(build);
			LOGGER.info("Successfully loaded plugin: " + build.getName());
			return true;
		} catch (InstantiationException e) {
			LOGGER.error("Failed to instantiate " + build.getName() + ": " + e.getMessage());
			return false;
		} catch (IllegalAccessException e) {
			LOGGER.error("Cannot access plugin " + build.getName() + ": " + e.getMessage());
			return false;
		} catch (ClassNotFoundException e) {
			LOGGER.error("Incorrect/missing absolute class path for plugin " + build.getName() + ": " + e.getMessage());
			return false;
		}
	}

	public static boolean loadPlugin(String name, String description, int version, String author,
			String absoluteClassPath, byte[] classData) {
		Plugin build = new Plugin(name, description, version, author, absoluteClassPath, classData);
		return loadPlugin(build);
	}

	public static boolean pluginExists(String name) {
		for (Plugin plugin : _loadedPlugins) {
			if (plugin.getName().equals(name)) {
				return true;
			}
		}
		return false;
	}

	public static boolean unloadPlugin(String name) {
		for (Plugin plugin : _loadedPlugins) {
			if (plugin.getName().equals(name)) {
				_loadedPlugins.remove(plugin);
				// pray that this implementation of the JVM will GC the class definition
				System.gc();
				return true;
			}
		}
		LOGGER.warn("Attempted to remove nonexistant plugin " + name);
		return false;
	}

	public static ArrayList<Plugin> getLoadedPlugins() {
		return _loadedPlugins;
	}
}
