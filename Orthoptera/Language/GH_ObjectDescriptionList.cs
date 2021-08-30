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
    internal class GH_ObjectDescriptionList
    {
        private static string XmlName => "DocumentObjects";

        private List<GH_ObjectDescription> _descriptions = new List<GH_ObjectDescription>();

        internal GH_ObjectDescriptionList(IList<IGH_ObjectProxy> proxies)
        {
            foreach (var proxy in proxies)
            {
                _descriptions.Add(GH_ObjectDescription.CreateFromProxy(proxy));
            }
        }

        internal void WriteXml(string fileLocation)
        {
            string fileName = fileLocation.Split('\\').Last();
            string sha = UnsafeHelper.HashString(fileName);

            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement(XmlName);
            element.SetAttribute("HASH", sha);
            foreach (var desc in _descriptions)
            {
                element.AppendChild(desc.ToXml(doc));
            }
            doc.AppendChild(element);

            doc.Save(fileLocation);
        }
    }
}
