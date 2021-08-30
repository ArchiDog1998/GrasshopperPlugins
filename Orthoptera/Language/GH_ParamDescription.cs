/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Orthoptera.Language
{
    internal class GH_ParamDescription
    {
        private static string XmlName => "Param";
        internal string Name { get; set; }
        internal string NickName { get; set; }
        internal string Description { get; set; }

        internal GH_ParamDescription(IGH_Param param)
        {
            this.Name = param.Name;
            this.NickName = param.NickName;
            this.Description = param.Description;
        }

        public GH_ParamDescription(XmlElement element)
        {
            this.Name = element.GetAttribute(nameof(this.Name));
            this.NickName = element.GetAttribute(nameof(this.NickName));
            this.Description = element.GetAttribute(nameof(this.Description));
        }

        internal XmlElement ToXml(XmlDocument doc)
        {
            XmlElement xmlElement = doc.CreateElement(XmlName);
            xmlElement.SetAttribute(nameof(this.Name), this.Name);
            xmlElement.SetAttribute(nameof(this.NickName), this.NickName);
            xmlElement.SetAttribute(nameof(this.Description), this.Description);
            return xmlElement;
        }

        public bool Equals(GH_ParamDescription other)
        {
            return this.Name == other.NickName && this.NickName == other.NickName && this.Description == other.Description;
        }
    }
}
