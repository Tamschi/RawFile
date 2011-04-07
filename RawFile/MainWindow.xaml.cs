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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace RawFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            mainTree.ItemsSource = App.Args.Select<string, UIElement>(OpenFile);
        }

        private UIElement OpenFile(string path)
        {
            var fileItem = new TreeViewItem()
            {
                IsExpanded = true,
                ItemsSource = new UIElement[] { PluginManager.GetUIElement(path.ToLowerInvariant(), path) }
            };
            var closeButton = new Button() { Content = "Close" };
            var fileItemHeaderPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            fileItemHeaderPanel.Children.Add(new Label() { Content = path });
            fileItemHeaderPanel.Children.Add(closeButton);
            fileItem.Header = fileItemHeaderPanel;
            closeButton.Click += (sender, e) =>
                {
                    mainTree.ItemsSource = mainTree.Items.OfType<TreeViewItem>().Where(x => x != fileItem);
                };
            return fileItem;
        }

        private void openFileClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                mainTree.ItemsSource = mainTree.Items.OfType<TreeViewItem>().Concat(dialog.FileNames.Select(OpenFile));
            }
        }

        private void settingsClick(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }
    }
}
