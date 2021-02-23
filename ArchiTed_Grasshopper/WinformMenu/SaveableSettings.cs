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
        #region Language
        public static List<string> ComponentLanguages { get; } = new List<string> { "English", "中文" };

        public static event EventHandler LanguageChanged;

        public static int language
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(language),
                System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "zh-CN" ? 1 : 0);
            set 
            {
                Grasshopper.Instances.Settings.GetValue(nameof(language), value);
                LanguageChanged.Invoke(null, new EventArgs());
            }
        }

        public static string GetTransLation(string[] strs)
        {
            try
            {
                string result = strs[language];
                if (result != "")
                    return result;
                else
                    return strs[0];
            }
            catch (ArgumentOutOfRangeException)
            {
                return strs[0];
            }
        }
        #endregion

        public Dictionary<T, object> DefaultDictionary { get; }

        public SaveableSettings(Dictionary<T, object> defaults)
        {
            DefaultDictionary = defaults;
        }

        public object GetProperty(T property)
        {
            object @default;
            if (!DefaultDictionary.TryGetValue(property, out @default)) throw new Exception(property.ToString() + " doesn't have a default!");

            if (@default is bool)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (bool)@default);
            else if (@default is byte)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (byte)@default);
            else if (@default is DateTime)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (DateTime)@default);
            else if (@default is double)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (double)@default);
            else if (@default is int)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (int)@default);
            else if (@default is string)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (string)@default);
            else if (@default is Point)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (Point)@default);
            else if (@default is Color)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (Color)@default);
            else if (@default is Rectangle)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (Rectangle)@default);
            else if (@default is Size)
                return Grasshopper.Instances.Settings.GetValue(property.ToString(), (Size)@default);

            throw new Exception(property.ToString() + " is a invalid type!");
        }

        public void SetProperty(T property, object value)
        {
            if (value is bool)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (bool)value);
            else if (value is byte)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (byte)value);
            else if (value is DateTime)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (DateTime)value);
            else if (value is double)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (double)value);
            else if (value is int)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (int)value);
            else if (value is string)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (string)value);
            else if (value is Point)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (Point)value);
            else if (value is Color)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (Color)value);
            else if (value is Rectangle)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (Rectangle)value);
            else if (value is Size)
                Grasshopper.Instances.Settings.SetValue(property.ToString(), (Size)value);
        }
    }
}
