/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Orthoptera.Language
{
    public class GH_ObjectDescriptionTable : Xmlable
    {
        private List<GH_ObjectDescription> _descriptions = new List<GH_ObjectDescription>();

        protected override string XmlName => "DocumentObjects";

        public GH_ObjectDescription this[int i] => _descriptions[i];

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

        public GH_ObjectDescriptionTable(IList<IGH_ObjectProxy> proxies)
        {
            foreach (var proxy in proxies)
            {
                _descriptions.Add(GH_ObjectDescription.CreateFromProxy(proxy));
            }
        }


        public override void ChangeFromXml(XmlElement element)
        {
            _descriptions.Clear();
            foreach (var obj in element.ChildNodes)
            {
                XmlNode node = (XmlNode)obj;
                _descriptions.Add(new GH_ObjectDescription((XmlElement)node));
            }
        }

        protected override void ToXml(ref XmlElement element, XmlDocument doc)
        {
            foreach (var desc in _descriptions)
            {
                element.AppendChild(desc.WriteXml(doc));
            }
        }
    }
}
