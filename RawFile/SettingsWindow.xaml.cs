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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RawFileInterface;

namespace RawFile
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// <para>The settings window display information and settings about loaded plugins.</para>
    /// </summary>
    public partial class SettingsWindow : Window
    {
        IPlugin[] plugins;

        public SettingsWindow()
        {
            InitializeComponent();

            plugins = PluginManager.Plugins.ToArray();

            pluginList.ItemsSource = plugins.Select(x => new ListViewItem() { Content = x.UniqueName });
        }

        private void pluginList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settingsPanel.Children.Clear();

            var settingsList = new List<StackPanel>();
            foreach (var plugin in plugins.Where((x, i) => pluginList.Items.OfType<ListViewItem>().ElementAt(i).IsSelected))
            {
                var settingsStack = new StackPanel();
                settingsStack.Children.Add(new Label() { Content = plugin.UniqueName });
                try
                {
                    settingsStack.Children.Add(plugin.Settings ?? new Label() { Content = "No settings!", Foreground = Brushes.DarkRed });
                }
                catch (Exception ex)
                {
                    settingsStack.Children.Add(new Label() { Content = ex.ToString(), Foreground = Brushes.DarkRed });
                }

                settingsList.Add(settingsStack);
            }

            for (int i = 0; i < settingsList.Count; i++)
            {
                settingsPanel.Children.Add(settingsList[i]);
                if (i < settingsList.Count - 1)
                {
                    settingsPanel.Children.Add(new Separator());
                }
            }
        }
    }
}
