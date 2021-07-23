/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper
{
    public static class GH_SettingsServerExtention
    {
        #region Major
        internal static object PropResetValue<T>(this GH_SettingsServer server, SettingsPreset<T> preset) where T : Enum
        {
            PropSetValue(server, preset, preset.Default);
            return preset.Default;
        }

        internal static void PropSetValue<T>(this GH_SettingsServer server, SettingsPreset<T> preset, object value) where T : Enum
        {
            //Check if Value is not changed.
            if (PropGetValue(server, preset).Equals(value)) return;

            //Find Key.
            string key = preset.Name.GetType().Name + "." + preset.Name.ToString();

            //Set value.
            server.SetValue(key, value);

            //ValueChanged.
            if (preset.ValueChanged != null)
                preset.ValueChanged.Invoke(value);
        }

        internal static object PropGetValue<T>(this GH_SettingsServer server, SettingsPreset<T> preset) where T : Enum
        {
            //Get the default
            object @default = preset.Default;

            //Find Key.
            string key = preset.Name.GetType().Name + "." + preset.Name.ToString();

            //Get Value.
            return server.GetValue(key, @default);
        }
        #endregion

        public static void SetValue(this GH_SettingsServer server, string key, object value)
        {
            //Set value.
            object @default = GetValue(server, key, value);

            if (@default is bool)
                server.SetValue(key, (bool)value);
            else if (@default is byte)
                server.SetValue(key, (byte)value);
            else if (@default is DateTime)
                server.SetValue(key, (DateTime)value);
            else if (@default is double)
                server.SetValue(key, (double)value);
            else if (@default is int)
                server.SetValue(key, (int)value);
            else if (@default is string)
                server.SetValue(key, (string)value);
            else if (@default is Point)
                server.SetValue(key, (Point)value);
            else if (@default is Color)
                server.SetValue(key, (Color)value);
            else if (@default is Rectangle)
                server.SetValue(key, (Rectangle)value);
            else if (@default is Size)
                server.SetValue(key, (Size)value);
            else if (@default is Guid)
                server.SetValue(key, (Guid)value);
            else if (@default is Font)
                server.SetValue(key, (Font)value);
            else if (@default is IEnumerable<Guid>)
                server.SetValue(key, (IEnumerable)value);
            else if (@default is IDictionary)
                server.SetValue(key, (IDictionary)value);
            else
                throw new Exception(nameof(value) + " is not the right type!");
        }

        public static object GetValue(this GH_SettingsServer server, string key, object @default)
        {
            //Get value
            if (@default is bool)
                return server.GetValue(key, (bool)@default);
            else if (@default is byte)
                return server.GetValue(key, (byte)@default);
            else if (@default is DateTime)
                return server.GetValue(key, (DateTime)@default);
            else if (@default is double)
                return server.GetValue(key, (double)@default);
            else if (@default is int)
                return server.GetValue(key, (int)@default);
            else if (@default is string)
                return server.GetValue(key, (string)@default);
            else if (@default is Point)
                return server.GetValue(key, (Point)@default);
            else if (@default is Color)
                return server.GetValue(key, (Color)@default);
            else if (@default is Rectangle)
                return server.GetValue(key, (Rectangle)@default);
            else if (@default is Size)
                return server.GetValue(key, (Size)@default);
            else if (@default is Guid)
                return server.GetValue(key, (Guid)@default);
            else if (@default is Font)
                return server.GetValue(key, (Font)@default);
            else if (@default is IEnumerable<Guid>)
                return server.GetValue(key, (IEnumerable)@default);
            else if (@default is IDictionary)
                return server.GetValue(key, (IDictionary)@default);
            throw new Exception(key + " is a invalid type!");
        }

        #region Font
        public static Font GetValue(this GH_SettingsServer server, string key, Font @default)
        {
            string fontStr = server.GetValue(key, string.Empty);
            if (fontStr == string.Empty)
            {
                return @default;
            }
            else
            {
                return GH_FontServer.StringToFont(fontStr);
            }
        }
        public static void SetValue(this GH_SettingsServer server, string key, Font value)
        {
            server.SetValue(key, GH_FontServer.FontToString(value));
        }
        #endregion

        #region Guid
        public static Guid GetValue(this GH_SettingsServer server, string key, Guid @default)
        {
            bool isSuccess = true;

            //Find bytes.
            byte[] bytes = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                //get byte.
                byte _byte = server.GetValue(key + i.ToString("D2"), default(byte));

                //Check whether succeed.
                if (_byte == default(byte))
                {
                    isSuccess = false;
                    break;
                }
                else
                {
                    bytes[i] = _byte;
                }
            }

            return isSuccess ? new Guid(bytes) : (Guid)@default;
        }

        public static void SetValue(this GH_SettingsServer server, string key, Guid value)
        {
            //Get all bytes.
            byte[] bytes = ((Guid)value).ToByteArray();

            //Save each bytes.
            for (int i = 0; i < 16; i++)
            {
                server.SetValue(key + i.ToString("D2"), bytes[i]);
            }
        }
        #endregion

        #region IEnumerable<Guid>
        public static IEnumerable GetValue(this GH_SettingsServer server, string key, IEnumerable @default)
        {
            //Get count;
            int count = server.GetValue(key + "Count", 0);
            if (count == 0) return @default;

            object[] result = new object[count];
            for (int i = 0; i < count; i++)
            {
                object temp = server.GetValue(key + i.ToString("D10"), default(object));
                result[i] = temp;
            }
            return result;
        }

        public static void SetValue(this GH_SettingsServer server, string key, IEnumerable value)
        {
            //Remove Value
            int beforeCount = server.GetValue(key + "Count", 0);
            for (int j = 0; j < beforeCount; j++)
            {
                server.DeleteValue(key + j.ToString("D10"));
            }

            //Set Value
            int count = value.Cast<object>().Count();
            server.SetValue(key + "Count", count);
            for (int i = 0; i < count; i++)
            {
                server.SetValue(key + i.ToString("D10"), value.Cast<object>().ElementAt(i));
            }
        }
        #endregion

        #region Dictionary
        public static IDictionary GetValue(this GH_SettingsServer server, string key, IDictionary @default)
        {
            //Get count;
            int count = server.GetValue(key + "Count", 0);
            if (count == 0) return @default;

            Dictionary<object, object> result = new Dictionary<object, object>();
            for (int i = 0; i < count; i++)
            {
                //Get every guid.
                object valueKey = server.GetValue(key + "Key" + i.ToString("D10"), default(object));
                object Value = server.GetValue(key + "Value" + i.ToString("D10"), default(object));
                result[valueKey] = Value;
            }
            return result;
        }

        public static void SetValue(this GH_SettingsServer server, string key, IDictionary value)
        {
            //Remove Value
            int beforeCount = server.GetValue(key + "Count", 0);
            for (int k = 0; k < beforeCount; k++)
            {
                server.DeleteValue(key + "Key" + k.ToString("D10"));
                server.DeleteValue(key + "Value" + k.ToString("D10"));
            }

            //Set Value
            int count = value.Count;
            server.SetValue(key + "Count", count);

            int i = 0;
            foreach (var item in value.Keys)
            {
                server.SetValue(key + "Key" + i.ToString("D10"), item);
                i++;
            }

            int j = 0;
            foreach (var item in value.Values)
            {
                server.SetValue(key + "Value" + j.ToString("D10"), item);
                j++;
            }
        }
        #endregion
    }
}