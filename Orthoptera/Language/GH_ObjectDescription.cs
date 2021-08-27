/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Grasshopper.Kernel;

namespace Orthoptera.Language
{
    public class GH_ObjectDescription : Xmlable
    {
        protected override string XmlName => "Object";
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Description { get; set; }
        public string ObjectFullName { get; set; }

        public List<GH_ObjectDescription> InputParamDescription { get; } = new List<GH_ObjectDescription>();
        public List<GH_ObjectDescription> OutputParamDescription { get; } = new List<GH_ObjectDescription>();

        private GH_ObjectDescription(IGH_DocumentObject obj)
        {
            this.Name = obj.Name;
            this.NickName = obj.NickName;
            this.Description = obj.Description;
            this.ObjectFullName = obj.GetType().FullName;

            if (obj is GH_Component)
            {
                GH_Component com = (GH_Component)obj;
                foreach (var input in com.Params.Input)
                {
                    this.InputParamDescription.Add(new GH_ObjectDescription(input));
                }
                foreach (var output in com.Params.Output)
                {
                    this.OutputParamDescription.Add(new GH_ObjectDescription(output));
                }
            }
        }

        private GH_ObjectDescription(IGH_ObjectProxy proxy)
            :this(proxy.CreateInstance())
        {
        }
        public static GH_ObjectDescription CreateFromProxy(IGH_ObjectProxy proxy)
        {
            if (proxy.Kind == GH_ObjectType.CompiledObject)
            {
                return new GH_ObjectDescription(proxy);
            }
            return null;
        }

        #region XML
        public GH_ObjectDescription(XmlElement element)
        {
            ChangeFromXml(element);
        }



        protected override void ToXml(ref XmlElement xmlElement, XmlDocument doc)
        {
            xmlElement.SetAttribute(nameof(this.ObjectFullName), this.ObjectFullName);
            xmlElement.SetAttribute(nameof(this.Name), this.Name);
            xmlElement.SetAttribute(nameof(this.NickName), this.NickName);
            xmlElement.SetAttribute(nameof(this.Description), this.Description);

            if(this.InputParamDescription.Count > 0)
            {
                XmlElement inputXmlElement = doc.CreateElement(nameof(this.InputParamDescription));
                
                foreach (var input in this.InputParamDescription)
                {
                    XmlElement inputEle = input.WriteXml(doc);
                    inputXmlElement.AppendChild(inputEle);
                }
                xmlElement.AppendChild(inputXmlElement);
            }

            if (this.OutputParamDescription.Count > 0)
            {
                XmlElement outputXmlElement = doc.CreateElement(nameof(this.OutputParamDescription));

                foreach (var output in this.InputParamDescription)
                {
                    XmlElement outputEle = output.WriteXml(doc);
                    outputXmlElement.AppendChild(outputEle);
                }
                xmlElement.AppendChild(outputXmlElement);
            }
        }

        public override void ChangeFromXml(XmlElement element)
        {
            this.ObjectFullName = element.GetAttribute(nameof(this.ObjectFullName));
            this.Name = element.GetAttribute(nameof(this.Name));
            this.NickName = element.GetAttribute(nameof(this.NickName));
            this.Description = element.GetAttribute(nameof(this.Description));

            foreach (var obj in element.ChildNodes)
            {
                XmlNode node = (XmlNode)obj;
                if (node.Name == nameof(this.InputParamDescription))
                {
                    this.InputParamDescription.Add(new GH_ObjectDescription((XmlElement)node));
                }
                else if (node.Name == nameof(this.OutputParamDescription))
                {
                    this.OutputParamDescription.Add(new GH_ObjectDescription((XmlElement)node));
                }
            }
        }
        #endregion
    }
}
