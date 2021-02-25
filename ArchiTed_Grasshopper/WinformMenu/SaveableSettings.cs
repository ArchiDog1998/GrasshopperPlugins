/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;

namespace ArchiTed_Grasshopper
{
    public class SaveableSettings<T> where T : Enum
    {
        public Dictionary<T, object> DefaultDictionary { get; }
        public GH_SettingsServer SettingsServer { get;}
        public SaveableSettings(Dictionary<T, object> defaults, GH_SettingsServer server)
        {
            DefaultDictionary = defaults;
            SettingsServer = server;
        }

        public object GetProperty(T property)
        {
            object @default;
            if (!DefaultDictionary.TryGetValue(property, out @default)) throw new Exception(property.ToString() + " doesn't have a default!");

            if (@default is bool)
                return SettingsServer.GetValue(property.ToString(), (bool)@default);
            else if (@default is byte)
                return SettingsServer.GetValue(property.ToString(), (byte)@default);
            else if (@default is DateTime)
                return SettingsServer.GetValue(property.ToString(), (DateTime)@default);
            else if (@default is double)
                return SettingsServer.GetValue(property.ToString(), (double)@default);
            else if (@default is int)
                return SettingsServer.GetValue(property.ToString(), (int)@default);
            else if (@default is string)
                return SettingsServer.GetValue(property.ToString(), (string)@default);
            else if (@default is Point)
                return SettingsServer.GetValue(property.ToString(), (Point)@default);
            else if (@default is Color)
                return SettingsServer.GetValue(property.ToString(), (Color)@default);
            else if (@default is Rectangle)
                return SettingsServer.GetValue(property.ToString(), (Rectangle)@default);
            else if (@default is Size)
                return SettingsServer.GetValue(property.ToString(), (Size)@default);
            else if (@default is Guid)
                return SettingsServer.GetValue(property.ToString(), (Guid)@default);
            else if(@default is IEnumerable<Guid>)
                return SettingsServer.GetValue(property.ToString(), (IEnumerable<Guid>)@default);


            throw new Exception(property.ToString() + " is a invalid type!");
        }

        public void SetProperty(T property, object value)
        {
            if (value is bool)
                SettingsServer.SetValue(property.ToString(), (bool)value);
            else if (value is byte)
                SettingsServer.SetValue(property.ToString(), (byte)value);
            else if (value is DateTime)
                SettingsServer.SetValue(property.ToString(), (DateTime)value);
            else if (value is double)
                SettingsServer.SetValue(property.ToString(), (double)value);
            else if (value is int)
                SettingsServer.SetValue(property.ToString(), (int)value);
            else if (value is string)
                SettingsServer.SetValue(property.ToString(), (string)value);
            else if (value is Point)
                SettingsServer.SetValue(property.ToString(), (Point)value);
            else if (value is Color)
                SettingsServer.SetValue(property.ToString(), (Color)value);
            else if (value is Rectangle)
                SettingsServer.SetValue(property.ToString(), (Rectangle)value);
            else if (value is Size)
                SettingsServer.SetValue(property.ToString(), (Size)value);
            else if (value is Guid)
                SettingsServer.SetValue(property.ToString(), (Guid)value);
            else if(value is IEnumerable<Guid>)
                SettingsServer.SetValue(property.ToString(), (IEnumerable<Guid>)value);

        }
    }
}
