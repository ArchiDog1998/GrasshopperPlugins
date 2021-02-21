/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGlasses.WinformMenu
{
    /// <summary>
    /// A class to hold the setting param.
    /// </summary>
    public static class Settings
    {
        public static bool ChangeSettingValue(PropertyName name, bool change)
        { 
            if (change)
                Grasshopper.Instances.Settings.SetValue(name.ToString(),
                    !Grasshopper.Instances.Settings.GetValue(name.ToString(), true));
            return Grasshopper.Instances.Settings.GetValue(name.ToString(), true);
        }
    }

    public enum PropertyName
    {
        IsFixCategoryIcon,

    }
}
