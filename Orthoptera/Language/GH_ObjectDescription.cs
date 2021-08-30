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
    internal class GH_ObjectDescription
    {
        private static string XmlName => "Object";
        internal string Name { get; set; }
        internal string NickName { get; set; }
        internal string Description { get; set; }
        internal string ObjectFullName { get; set; }
        internal string Translator { get; set; } = "";

        internal List<GH_ParamDescription> InputParamDescription { get; } = new List<GH_ParamDescription>();
        internal List<GH_ParamDescription> OutputParamDescription { get; } = new List<GH_ParamDescription>();

        public bool Equals(GH_ObjectDescription other)
        {
            if (!(this.Name == other.NickName && this.NickName == other.NickName && this.Description == other.Description)) return false;
            if (!(this.InputParamDescription.Count == other.InputParamDescription.Count && this.OutputParamDescription.Count == other.OutputParamDescription.Count)) return false;
            for (int i = 0; i < this.InputParamDescription.Count; i++)
            {
                if (!this.InputParamDescription[i].Equals(other.InputParamDescription[i])) return false;
            }
            for (int i = 0; i < this.OutputParamDescription.Count; i++)
            {
                if (!this.OutputParamDescription[i].Equals(other.OutputParamDescription[i])) return false;
            }
            return true;
        }

        public GH_ObjectDescription(IGH_DocumentObject obj)
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
                    this.InputParamDescription.Add(new GH_ParamDescription(input));
                }
                foreach (var output in com.Params.Output)
                {
                    this.OutputParamDescription.Add(new GH_ParamDescription(output));
                }
            }
        }

        private GH_ObjectDescription(IGH_ObjectProxy proxy)
        {
            this.Name = proxy.Desc.Name;
            this.NickName = proxy.Desc.NickName;
            this.Description = proxy.Desc.Description;

            switch (proxy.Kind)
            {
                case GH_ObjectType.CompiledObject:
                    this.ObjectFullName = proxy.Type.FullName;
                    break;
                case GH_ObjectType.UserObject:
                    this.ObjectFullName = proxy.Location.Split('\\').Last();
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
                    this.InputParamDescription.Add(new GH_ParamDescription(input));
                }
                foreach (var output in com.Params.Output)
                {
                    this.OutputParamDescription.Add(new GH_ParamDescription(output));
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
        internal GH_ObjectDescription(XmlElement element)
        {
            this.Translator = element.GetAttribute(nameof(this.Translator));
            this.Name = element.GetAttribute(nameof(this.Name));
            this.NickName = element.GetAttribute(nameof(this.NickName));
            this.Description = element.GetAttribute(nameof(this.Description));
            this.ObjectFullName = element.GetAttribute(nameof(this.ObjectFullName));


            foreach (var obj in element.ChildNodes)
            {
                XmlNode node = (XmlNode)obj;
                if (node.Name == nameof(this.InputParamDescription))
                {
                    this.InputParamDescription.Add(new GH_ParamDescription((XmlElement)node));
                }
                else if (node.Name == nameof(this.OutputParamDescription))
                {
                    this.OutputParamDescription.Add(new GH_ParamDescription((XmlElement)node));
                }
            }
        }

        internal XmlElement ToXml(XmlDocument doc)
        {
            XmlElement xmlElement = doc.CreateElement(XmlName);

            xmlElement.SetAttribute(nameof(this.Translator), this.Translator);
            xmlElement.SetAttribute(nameof(this.Name), this.Name);
            xmlElement.SetAttribute(nameof(this.NickName), this.NickName);
            xmlElement.SetAttribute(nameof(this.Description), this.Description);
            xmlElement.SetAttribute(nameof(this.ObjectFullName), this.ObjectFullName);


            if (this.InputParamDescription.Count > 0)
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
