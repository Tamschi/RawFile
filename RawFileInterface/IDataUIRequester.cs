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
using System.IO;
using System.Windows;

namespace RawFileInterface
{
    /// <summary>
    /// Used for plugins that relay data to other plugins.
    /// </summary>
    public interface IDataUIRequester : IPlugin
    {
        /// <summary>
        /// Used for relaying data to other plugins.
        /// <para>Parameters:</para>
        /// <para>1. The data id for finding a plugin that can process the data.</para>
        /// <para>2. The path to the file containing the data (if available).</para>
        /// <para>3. A byte array containing the data.</para>
        /// <para>4. The offset of the data in data.</para>
        /// <para>5. The number of bytes in data that should be processed.</para>
        /// </summary>
        Func<string, string, byte[], long, long, UIElement> DataUIRequest { set; }
    }
}
