/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;

namespace ArchiTed_Grasshopper
{

    public struct SettingsPreset<T> where T : Enum
    {
        public T Name { get; }
        public object Default { get; }
        public Action<object> ValueChanged { get; }
        public Type Type { get; }
        public SettingsPreset(T name, object @default, Action<object> valugChanged = null)
        {
            this.Name = name;
            this.Default = @default;
            this.ValueChanged = valugChanged;
            this.Type = @default.GetType();
        }
    }

    public class SaveableSettings<T> where T : Enum
    {
        public SettingsPreset<T>[] PropertiesSet { get; }
        public GH_SettingsServer SettingsServer { get;}
        public SaveableSettings(SettingsPreset<T>[] defaults, GH_SettingsServer server)
        {
            PropertiesSet = defaults;
            SettingsServer = server;
        }
        private SettingsPreset<T> FindProperty(T property)
        {
            IEnumerable<SettingsPreset<T>> result = PropertiesSet.Where((item) => item.Name.ToString() == property.ToString());

            if (result.Count() == 0) throw new Exception(property.ToString() + " haven't been set a " + nameof(SettingsPreset<T>));

            return result.ElementAt(0);
        }

        public object GetProperty(T property)
        {
            return SettingsServer.PropGetValue(FindProperty(property));
        }

        public void ResetProperty(T property)
        {
            SettingsServer.PropResetValue(FindProperty(property));
        }

        public void SetProperty(T property, object value)
        {
            SettingsServer.PropSetValue(FindProperty(property), value);
        }
    }
}
