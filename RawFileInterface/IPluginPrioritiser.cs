﻿/* © 2011 Tamme Schichler (tammeschichler@googlemail.com)
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

namespace RawFileInterface
{
    /// <summary>
    /// Used for plugins that change the priority of plugins for data and file ids.
    /// </summary>
    public interface IPluginPrioritiser : IPlugin
    {
        /// <summary>
        /// The plugin can invoke this delegate to change the priority of the plugins for a data id.
        /// <para>Parameters:</para>
        /// <para>1. The data id, for wich the priority should be changed.</para>
        /// <para>2. The plugins for the data id ordered by their new priority (descending).</para>
        /// </summary>
        Action<string, IDataPlugin[]> DataIdPluginPriorityChange { set; }

        /// <summary>
        /// The plugin can invoke this delegate to change the priority of the plugins for a file id.
        /// <para>Parameters:</para>
        /// <para>1. The file id, for wich the priority should be changed.</para>
        /// <para>2. The plugins for the file id ordered by their new priority (descending).</para>
        /// </summary>
        Action<string, IFilePlugin[]> FileIdPluginPriorityChange { set; }
    }
}
