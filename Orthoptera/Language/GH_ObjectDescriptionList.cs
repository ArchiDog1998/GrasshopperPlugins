/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Orthoptera.Language
{
    public class GH_ObjectDescriptionList
    {
        private List<GH_ObjectDescription> _descriptions = new List<GH_ObjectDescription>();
        private string _assmeblyName;
        private string XmlName => "DocumentObjects";

        public GH_ObjectDescription this[string objectFullName] {
            get
            {
                foreach (var item in _descriptions)
                {
                    if (item.ObjectFullName == objectFullName)
                        return item;
                }
                return null;
            }
        }

        internal GH_ObjectDescriptionList(IList<IGH_ObjectProxy> proxies, string assemblyName)
        {
            this._assmeblyName = assemblyName;
            foreach (var proxy in proxies)
            {
                _descriptions.Add(GH_ObjectDescription.CreateFromProxy(proxy));
            }
        }


        internal void ReadXml(CultureInfo info)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Grasshopper.Folders.AppDataFolder + info.Name + "_" + _assmeblyName + ".xml");
            XmlElement element = (XmlElement)doc.ChildNodes[0];
            _descriptions.Clear();
            foreach (var obj in element.ChildNodes)
            {
                XmlNode node = (XmlNode)obj;
                _descriptions.Add(new GH_ObjectDescription((XmlElement)node));
            }
        }

        internal void WriteXml(string pathAndName)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement(XmlName);
            element.SetAttribute("Assembly", this._assmeblyName);
            foreach (var desc in _descriptions)
            {
                element.AppendChild(desc.ToXml(doc));
            }
            doc.AppendChild(element);

            doc.Save(pathAndName + _assmeblyName + ".xml");
        }
    }
}
