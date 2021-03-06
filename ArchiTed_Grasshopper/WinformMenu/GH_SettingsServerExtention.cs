﻿/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
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
            object @default = PropGetValue(server, preset);
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
                server.SetValue(key, (IEnumerable<Guid>)value);
            else
                throw new Exception(nameof(value) + " is not the right type!");

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
                return server.GetValue(key, (IEnumerable<Guid>)@default);

            throw new Exception(preset.Name.ToString() + " is a invalid type!");
        }
        #endregion

        #region Font
        public static Font GetValue(this GH_SettingsServer server, string key, Font @default)
        {
            string fontStr = server.GetValue(key, string.Empty);
            if(fontStr == string.Empty)
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
        public static IEnumerable<Guid> GetValue(this GH_SettingsServer server, string key, IEnumerable<Guid> @default)
        {
            //Get count;
            int count = server.GetValue(key + "Count", 0);

            Guid[] result = new Guid[count];
            for (int i = 0; i < count; i++)
            {
                //Get every guid.
                Guid temp = server.GetValue(key + i.ToString("D10"), Guid.Empty);
                if (temp == Guid.Empty) throw new Exception(key + i.ToString("D10") + "is not found!");
                result[i] = temp;
            }
            return result;
        }

        public static void SetValue(this GH_SettingsServer server, string key, IEnumerable<Guid> value)
        {
            //Remove Value
            int beforeCount = server.GetValue(key + "Count", 0);
            for (int j = 0; j < beforeCount; j++)
            {
                server.DeleteValue(key + j.ToString("D10"));
            }

            //Set Value
            int count = value.Count();
            server.SetValue(key + "Count", count);
            for (int i = 0; i < count; i++)
            {
                server.SetValue(key + i.ToString("D10"), value.ElementAt(i));
            }
        }
        #endregion
    }
}