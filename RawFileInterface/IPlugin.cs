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
 * 
 * Additional permission under GNU GPL version 3 section 7
 * 
 * If you modify this Program, or any covered work, by linking or
 * combining it with a library (or a modified version of a library),
 * for wich no suitable alternative library covered by the GNU General
 * Public License exists and no such library can be created with reasonable
 * efforts, the licensors of this Program grant you additional permission
 * to convey the resulting work.
 * Corresponding Source for a non-source form of such a combination shall
 * include the source code for the parts of the library used, if permitted
 * under the library's license, as well as that of the covered work.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RawFileInterface
{
    /// <summary>
    /// A plugin for RawFile.
    /// <para>All plugins must implement this interface.</para>
    /// </summary>
    /// <remarks>
    /// Plugins shouldn't rely on being saved in a certain folder.
    /// <para>The assembly name of a plugin must end with "RawPlugin.dll".</para>
    /// <para>Plugins must reference System.Xaml, WindowsBase and PresentationCore to implement this interface.</para>
    /// </remarks>
    public interface IPlugin
    {
        /// <summary>
        /// The UIElement displayed in the settings dialog for this plugin.
        /// <para>It should contain information about the plugin and what it does,</para>
        /// <para>as well as all settings for the plugin.</para>
        /// <para></para>
        /// <para>Multiple calls to the accessor may return a new instance each,</para>
        /// <para>but aren't required to do so.</para>
        /// </summary>
        UIElement Settings { get; }

        /// <summary>
        /// The unique display name of the plugin.
        /// <para>The string must be a valid filename.</para>
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// The minimum interface version required for this plugin.
        /// </summary>
        int MinVersion { get; }
    }
}
