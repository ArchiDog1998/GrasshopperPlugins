/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Orthoptera.Language
{
    public abstract class Xmlable
    {
        protected abstract string XmlName { get; }

        public void ReadXml(XmlElement element)
        {
            if (element.Name != XmlName) throw new ArgumentOutOfRangeException($"Element is not a valid Element, its name must be {XmlName}!");
            ChangeFromXml(element);
        }
        public abstract void ChangeFromXml(XmlElement element);
        public XmlElement WriteXml(XmlDocument doc)
        {
            XmlElement xmlElement = doc.CreateElement(this.XmlName);
            this.ToXml(ref xmlElement, doc);
            return xmlElement;
        }
        protected abstract void ToXml(ref XmlElement element, XmlDocument doc);
    }
}
