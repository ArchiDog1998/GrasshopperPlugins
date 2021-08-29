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
    public class GH_ObjectDescription
    {
        protected string XmlName => "Object";
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ObjectFullName { get; set; }
        public bool IsComiledObject { get; set; }

        public List<GH_ObjectDescription> InputParamDescription { get; } = new List<GH_ObjectDescription>();
        public List<GH_ObjectDescription> OutputParamDescription { get; } = new List<GH_ObjectDescription>();


        private GH_ObjectDescription(IGH_Param param)
        {
            this.Name = param.Name;
            this.NickName = param.NickName;
            this.Description = param.Description;
        }

        private GH_ObjectDescription(IGH_ObjectProxy proxy)
        {
            this.Name = proxy.Desc.Name;
            this.NickName = proxy.Desc.NickName;
            this.Description = proxy.Desc.Description;
            this.Category = proxy.Desc.Category;
            this.SubCategory = proxy.Desc.SubCategory;

            switch (proxy.Kind)
            {
                case GH_ObjectType.CompiledObject:
                    this.ObjectFullName = proxy.Type.FullName;
                    this.IsComiledObject = true;
                    break;
                case GH_ObjectType.UserObject:
                    this.ObjectFullName = proxy.Location.Split('\\').Last();
                    this.IsComiledObject = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Proxy is not a valid proxy, please input a CompiledProxy or a UserObjectProxy.");
            }

            IGH_DocumentObject obj = proxy.CreateInstance();
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

        internal static GH_ObjectDescription CreateFromProxy(IGH_ObjectProxy proxy)
        {
            if (proxy.Kind == GH_ObjectType.CompiledObject || proxy.Kind == GH_ObjectType.UserObject)
            {
                return new GH_ObjectDescription(proxy);
            }
            return null;
        }

        #region XML
        public GH_ObjectDescription(XmlElement element)
        {
            this.ObjectFullName = element.GetAttribute(nameof(this.ObjectFullName));
            this.Name = element.GetAttribute(nameof(this.Name));
            this.NickName = element.GetAttribute(nameof(this.NickName));
            this.Description = element.GetAttribute(nameof(this.Description));
            this.IsComiledObject = bool.Parse(element.GetAttribute(nameof(this.IsComiledObject)));
            this.Category = element.GetAttribute(nameof(this.Category));
            this.SubCategory = element.GetAttribute(nameof(this.SubCategory));

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

        internal XmlElement ToXml(XmlDocument doc)
        {
            XmlElement xmlElement = doc.CreateElement(XmlName);

            xmlElement.SetAttribute(nameof(this.ObjectFullName), this.ObjectFullName);
            xmlElement.SetAttribute(nameof(this.Name), this.Name);
            xmlElement.SetAttribute(nameof(this.NickName), this.NickName);
            xmlElement.SetAttribute(nameof(this.Description), this.Description);
            xmlElement.SetAttribute(nameof(this.IsComiledObject), this.IsComiledObject.ToString());
            xmlElement.SetAttribute(nameof(this.Category), this.Category);
            xmlElement.SetAttribute(nameof(this.SubCategory), this.SubCategory);

            if(this.InputParamDescription.Count > 0)
            {
                XmlElement inputXmlElement = doc.CreateElement(nameof(this.InputParamDescription));
                
                foreach (var input in this.InputParamDescription)
                {
                    XmlElement inputEle = input.ToXml(doc);
                    inputXmlElement.AppendChild(inputEle);
                }
                xmlElement.AppendChild(inputXmlElement);
            }

            if (this.OutputParamDescription.Count > 0)
            {
                XmlElement outputXmlElement = doc.CreateElement(nameof(this.OutputParamDescription));

                foreach (var output in this.InputParamDescription)
                {
                    XmlElement outputEle = output.ToXml(doc);
                    outputXmlElement.AppendChild(outputEle);
                }
                xmlElement.AppendChild(outputXmlElement);
            }
            return xmlElement;
        }
        #endregion
    }
}
