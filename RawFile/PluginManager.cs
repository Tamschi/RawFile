/* © 2011 Tamme Schichler (tammeschichler@googlemail.com)
 * 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <http://www.gnu.org/licenses>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using RawFileInterface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Policy;
using System.Security;

namespace RawFile
{
    /// <summary>
    /// Manages all loaded plugins.
    /// </summary>
    static class PluginManager
    {
        /// <summary>
        /// The interface version supported by this class.
        /// </summary>
        const int Version = 0;

        public static List<IPlugin> Plugins { get; private set; }
        public static Dictionary<string, List<IDataPlugin>> DataPlugins { get; private set; }
        public static Dictionary<string, List<IFilePlugin>> FilePlugins { get; private set; }

        const string PluginPath = @".\Plugins";
        const string PluginPattern = @"*.RawPlugin.dll";

        /// <summary>
        /// Used to load all plugins in PluginPath and its subdirectories on startup.
        /// </summary>
        static internal void LoadPlugins()
        {
            Plugins = new List<IPlugin>();
            DataPlugins = new Dictionary<string, List<IDataPlugin>>();
            FilePlugins = new Dictionary<string, List<IFilePlugin>>();

            try
            {
                var pluginFiles = Directory.GetFiles(PluginPath, PluginPattern, SearchOption.AllDirectories).OrderBy(x => x).ToArray();
                var lateActivatedPlugins = new LinkedList<ILateActivatedPlugin>();

                foreach (var file in pluginFiles)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(file);
                        if (assembly != null)
                        {
                            foreach (var type in assembly.GetExportedTypes())
                            {
                                if (typeof(IPlugin).IsAssignableFrom(type))
                                {
                                    RegisterPlugin(type);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(file + Environment.NewLine + e.Message);
                    }
                }

                foreach (var lateActivatedPlugin in Plugins.OfType<ILateActivatedPlugin>())
                {
                    lateActivatedPlugin.ActivateLate();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static Assembly pluginDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            MessageBox.Show(args.RequestingAssembly.Location.ToString());
            MessageBox.Show(args.Name);
            throw new NotImplementedException();
        }

        static Assembly pluginDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //var domain = (AppDomain)sender;
            MessageBox.Show(sender.ToString());
            MessageBox.Show(args.RequestingAssembly.Location);
            MessageBox.Show(args.Name);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Instantiates and registers a new plugin.
        /// <para>Note that ILateActivatedPlugins are handled elsewhere.</para>
        /// </summary>
        /// <param name="pluginType">The Type of the new plugin.</param>
        private static void RegisterPlugin(Type pluginType)
        {
            if (!typeof(IPlugin).IsAssignableFrom(pluginType))
            {
                throw new ArgumentException("Plugin must implement IRawPlugin!", "type");
            }

            var plugin = (IPlugin)Activator.CreateInstance(pluginType);

            RegisterPluginInstance(plugin);
        }

        private static void RegisterPluginInstance(IPlugin plugin)
{
            var pluginType = plugin.GetType();

            if (plugin.MinVersion > Version)
            {
                MessageBox.Show(string.Format("The plugin {0} couldn't be loaded because it needs the interface version {1}.\nThe current interface version is {2}.", plugin.UniqueName, plugin.MinVersion, Version));
            }

            Plugins.Add(plugin);

            if (typeof(IPersistentPlugin).IsAssignableFrom(pluginType))
            {
                var persistentPlugin = (IPersistentPlugin)plugin;
                var persistentDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @".\PersistentPlugins\", plugin.UniqueName);

                if (!Directory.Exists(persistentDirectory))
                {
                    Directory.CreateDirectory(persistentDirectory);
                }

                persistentPlugin.PersistentDirectory = persistentDirectory;
            }

            if (typeof(IDataPlugin).IsAssignableFrom(pluginType))
            {
                var dataPlugin = (IDataPlugin)plugin;
                var ids = dataPlugin.DataIdentifiers;

                foreach (var id in ids.Distinct())
                {
                    if (!DataPlugins.ContainsKey(id))
                    {
                        DataPlugins.Add(id, new List<IDataPlugin>());
                    }

                    DataPlugins[id].Add(dataPlugin);
                }

                if (typeof(IDynamicDataPlugin).IsAssignableFrom(pluginType))
                {
                    var dynamicDataPlugin = (IDynamicDataPlugin)dataPlugin;

                    dynamicDataPlugin.DataIdentifiersChanged = () =>
                        {
                            DataPlugins = DataPlugins.ToDictionary(x => x.Key, x => x.Value.Where(y => y != dynamicDataPlugin).ToList()).Where(x => x.Value.Count != 0).ToDictionary(x => x.Key, x => x.Value);

                            var newIds = dynamicDataPlugin.DataIdentifiers;

                            foreach (var id in newIds.Distinct())
                            {
                                if (!DataPlugins.ContainsKey(id))
                                {
                                    DataPlugins.Add(id, new List<IDataPlugin>());
                                }

                                DataPlugins[id].Add(dynamicDataPlugin);
                            }
                        };
                }
            }

            if (typeof(IFilePlugin).IsAssignableFrom(pluginType))
            {
                var filePlugin = (IFilePlugin)plugin;
                var ids = filePlugin.FileIdentifiers;

                foreach (var id in ids.Distinct())
                {
                    if (!FilePlugins.ContainsKey(id))
                    {
                        FilePlugins.Add(id, new List<IFilePlugin>());
                    }

                    FilePlugins[id].Add(filePlugin);
                }

                if (typeof(IDynamicDataPlugin).IsAssignableFrom(pluginType))
                {
                    var dynamicFilePlugin = (IDynamicFilePlugin)filePlugin;

                    dynamicFilePlugin.DataIdentifiersChanged = () =>
                    {
                        FilePlugins = FilePlugins.ToDictionary(x => x.Key, x => x.Value.Where(y => y != dynamicFilePlugin).ToList()).Where(x => x.Value.Count != 0).ToDictionary(x => x.Key, x => x.Value);

                        var newIds = dynamicFilePlugin.FileIdentifiers;

                        foreach (var id in newIds.Distinct())
                        {
                            if (!DataPlugins.ContainsKey(id))
                            {
                                FilePlugins.Add(id, new List<IFilePlugin>());
                            }

                            FilePlugins[id].Add(dynamicFilePlugin);
                        }
                    };
                }
            }

            if (typeof(IDataUIRequester).IsAssignableFrom(pluginType))
            {
                var dataUIRequester = (IDataUIRequester)plugin;
                dataUIRequester.DataUIRequest = GetUIElement;
            }

            if (typeof(IFileUIRequester).IsAssignableFrom(pluginType))
            {
                var fileUIRequester = (IFileUIRequester)plugin;
                fileUIRequester.FileUIRequest = GetUIElement;
            }

            if (typeof(IPluginLoader).IsAssignableFrom(pluginType))
            {
                var pluginLoader = (IPluginLoader)plugin;
                pluginLoader.RegisterPlugin = (newPlugin) =>
                {
                    RegisterPluginInstance(newPlugin);
                    if (newPlugin is ILateActivatedPlugin)
                    {
                        ((ILateActivatedPlugin)newPlugin).ActivateLate();
                    }
                };
            }

            if (typeof(IPluginUnloader).IsAssignableFrom(pluginType))
            {
                var pluginUnloader = (IPluginUnloader)plugin;
                pluginUnloader.UnloadPlugin = UnloadPlugin;
            }

            if (typeof(IPluginRequester).IsAssignableFrom(pluginType))
            {
                var pluginRequester = (IPluginRequester)plugin;
                pluginRequester.PluginsRequest = () => { return Plugins.ToArray(); };
                pluginRequester.DataPluginsRequest = (string dataId) => { return DataPlugins[dataId].ToArray(); };
                pluginRequester.FilePluginsRequest = (string fileId) => { return FilePlugins[fileId].ToArray(); };
            }

            if (typeof(IIdRequester).IsAssignableFrom(pluginType))
            {
                var idRequester = (IIdRequester)plugin;
                idRequester.DataIdsRequest = () => { return DataPlugins.Keys.ToArray(); };
                idRequester.FileIdsRequest = () => { return FilePlugins.Keys.ToArray(); };
            }

            if (typeof(IPluginPrioritiser).IsAssignableFrom(pluginType))
            {
                var pluginPrioritiser = (IPluginPrioritiser)plugin;
                pluginPrioritiser.DataIdPluginPriorityChange = (dataId, plugins) =>
                    {
                        if (plugins.Length != dataId.Length)
                        {
                            throw new ArgumentException("Must specify all plugins for dataId.");
                        }
                        foreach (var dataPlugin in DataPlugins[dataId])
                        {
                            if (!plugins.Contains(dataPlugin))
                            {
                                throw new ArgumentException("Must specify all plugins for dataId.");
                            }
                        }

                        DataPlugins[dataId].Clear();
                        DataPlugins[dataId].AddRange(plugins);
                    };
                pluginPrioritiser.FileIdPluginPriorityChange = (fileId, plugins) =>
                {
                    if (plugins.Length != fileId.Length)
                    {
                        throw new ArgumentException("Must specify all plugins for fileId.");
                    }
                    foreach (var filePlugin in FilePlugins[fileId])
                    {
                        if (!plugins.Contains(filePlugin))
                        {
                            throw new ArgumentException("Must specify all plugins for fileId.");
                        }
                    }

                    FilePlugins[fileId].Clear();
                    FilePlugins[fileId].AddRange(plugins);
                };
            }
}

        /// <summary>
        /// Unloads a plugin and removes all its references in the data and file id dictionaries.
        /// </summary>
        /// <param name="plugin">The plugin to unload.</param>
        private static void UnloadPlugin(IPlugin plugin) //TODO: Find a way to safely unload the AppDomain.
        {
            if (Plugins.Contains(plugin))
            {
                Plugins.Remove(plugin);
            }

            var dataPlugin = plugin as IDataPlugin;
            if (dataPlugin != null)
            {
                foreach (var dataPluginListKey in DataPlugins.Keys.ToArray())
                {
                    var dataPluginList = DataPlugins[dataPluginListKey];
                    if (dataPluginList.Contains(dataPlugin))
                    {
                        dataPluginList.Remove(dataPlugin);
                    }
                    if (dataPluginList.Count == 0)
                    {
                        DataPlugins.Remove(dataPluginListKey);
                    }
                }
            }

            var filePlugin = plugin as IFilePlugin;
            if (filePlugin != null)
            {
                foreach (var filePluginListKey in FilePlugins.Keys.ToArray())
                {
                    var filePluginList = FilePlugins[filePluginListKey];
                    if (filePluginList.Contains(filePlugin))
                    {
                        filePluginList.Remove(filePlugin);
                    }
                    if (filePluginList.Count == 0)
                    {
                        FilePlugins.Remove(filePluginListKey);
                    }
                }
            }
        }

        /// <summary>
        /// Tries to find a plugin to process data.
        /// </summary>
        /// <param name="id">The data id for finding a plugin that can process the data.</param>
        /// <param name="filePath">The path to the file containing the data (if available).</param>
        /// <param name="data">A byte array containing the data.</param>
        /// <param name="offset">The offset of the data in data.</param>
        /// <param name="length">The number of bytes in data that should be processed.</param>
        /// <returns>A UIElement containing information about the data or null.</returns>
        static internal UIElement GetUIElement(string id, string filePath, byte[] data, long offset, long length)
        {
            UIElement uiElement;
            foreach (var plugin in DataPlugins.Where(x => id.EndsWith(x.Key)).OrderByDescending(x => x.Key.Length).SelectMany(x => x.Value))
            {
                try
                {
                    if (plugin.TryGetUIElement(out uiElement, id, filePath, data, offset, length))
                    {
                        return uiElement;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Plugin {0} failed:\n{1}", ((IPlugin)plugin).UniqueName, e.ToString()));
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to find a plugin to process a file.
        /// </summary>
        /// <param name="id">The file id for finding a plugin that can process the file.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A UIElement containing information about the file or null.</returns>
        static internal UIElement GetUIElement(string id, string filePath)
        {
            UIElement uiElement;
            foreach (var plugin in FilePlugins.Where(x => id.EndsWith(x.Key)).OrderByDescending(x => x.Key.Length).SelectMany(x => x.Value))
            {
                try
                {
                    if (plugin.TryGetUIElement(out uiElement, id, filePath))
                    {
                        return uiElement;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Plugin {0} failed:\n{1}", ((IPlugin)plugin).UniqueName, e.ToString()));
                }
            }

            return null;
        }
    }
}
