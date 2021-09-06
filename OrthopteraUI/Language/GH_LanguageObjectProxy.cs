/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Grasshopper;
using Grasshopper.Kernel;
using Microsoft.VisualBasic.CompilerServices;
using Rhino;

namespace OrthopteraUI.Language
{
    internal class GH_LanguageObjectProxy : IGH_ObjectProxy, Orthoptera.Language.IToXml
    {

        public string Location { get; }

        public Guid LibraryGuid { get; }

        public bool SDKCompliant { get; }

        public bool Obsolete { get; }

        public Type Type { get; }

        public GH_ObjectType Kind { get; }

        public Guid Guid { get; }

        public Bitmap Icon { get; }

        public IGH_InstanceDescription Desc { get; }

        public GH_Exposure Exposure { get; set; }

        public List<string[]> InputParams { get; internal set; }
        public List<string[]> OutputParams { get; internal set; }

        public string Translator { get; private set; }

        public List<DiagramForProxy> Diagrams { get; } = new List<DiagramForProxy>();

        public string EnglishCate { get; }
        public string EnglishSub { get; }


        //internal GH_LanguageObjectProxy(IGH_ObjectProxy proxy, string[] nameSet)
        //    :this(proxy)
        //{
        //    if (nameSet.Length != 6) throw new ArgumentOutOfRangeException($"{nameof(nameSet)}'s length should be 5.");
        //    this.Desc.Name = nameSet[0];
        //    this.Desc.NickName = nameSet[1];
        //    this.Desc.Description = nameSet[2];
        //    this.Desc.Category = nameSet[3];
        //    this.Desc.SubCategory = nameSet[4];
        //    this.Translator = nameSet[5];

        //    if (!string.IsNullOrEmpty(Translator))
        //    {
        //        this.Desc.Description += "\n----" + this.Translator;
        //    }
        //}

        internal GH_LanguageObjectProxy(IGH_ObjectProxy proxy)
        {
            this.Location = proxy.Location;
            this.LibraryGuid = proxy.Guid;
            this.SDKCompliant = proxy.SDKCompliant;
            this.Obsolete = proxy.Obsolete;
            this.Type = proxy.Type;
            this.Kind = proxy.Kind;
            this.Guid = proxy.Guid;
            this.Icon = proxy.Icon;
            this.Desc = proxy.Desc;
            this.Exposure = proxy.Exposure;

            this.EnglishCate = proxy.Desc.Category;
            this.EnglishSub = proxy.Desc.SubCategory;
        }

        private void Translate(ref IGH_DocumentObject obj, bool isTranslateParamNameAndNick)
        {
            obj.Name = Desc.Name;
            obj.NickName = Desc.NickName;
            obj.Description = Desc.Description;
            if (Desc.HasCategory)
                obj.Category = Desc.Category;
            if (Desc.HasSubCategory)
                obj.SubCategory = Desc.SubCategory;

            if(obj is GH_Component)
            {
                GH_Component com = obj as GH_Component;
                if(com.Params.Input.Count == InputParams.Count)
                {
                    for (int i = 0; i < com.Params.Input.Count; i++)
                    {
                        com.Params.Input[i].Description = InputParams[i][2];
                        if (isTranslateParamNameAndNick)
                        {
                            com.Params.Input[i].Name = InputParams[i][0];
                            com.Params.Input[i].NickName = InputParams[i][1];
                        }
                    }
                }
                else
                {
                    com.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"XML language file's record has a different count of inputs with this components'. Please check it.");
                }

                if(com.Params.Output.Count == OutputParams.Count)
                {
                    for (int i = 0; i < com.Params.Output.Count; i++)
                    {
                        com.Params.Output[i].Description = OutputParams[i][2];
                        if (isTranslateParamNameAndNick)
                        {
                            com.Params.Output[i].Name = OutputParams[i][0];
                            com.Params.Output[i].NickName = OutputParams[i][1];
                        }
                    }
                }
                else
                {
                    com.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"XML language file's record has a different count of outputs with this components'. Please check it.");
                }
            }
        }

        public IGH_DocumentObject CreateInstance()
        {
            try
            {
                switch (this.Kind)
                {
                    case GH_ObjectType.CompiledObject:
                        object objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(Type));
                        if (objectValue == null)
                        {
                            return null;
                        }
                        IGH_DocumentObject obj = objectValue as IGH_DocumentObject;
                        Translate(ref obj, true);
                        return obj;

                    case GH_ObjectType.UserObject:
                        GH_UserObject gH_UserObject = new GH_UserObject();
                        gH_UserObject.Path = Location;
                        gH_UserObject.ReadFromFile();

                        IGH_DocumentObject user = gH_UserObject.InstantiateObject();
                        Translate(ref user, false);
                        return user;
                    default:
                        return null;

                }

            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                IGH_DocumentObject result = null;
                ProjectData.ClearProjectError();
                return result;
            }
        }

        public IGH_ObjectProxy DuplicateProxy() => new GH_LanguageObjectProxy(this);

        public override string ToString()
        {
            return base.ToString() + this.Desc.Name + $"({this.Desc.NickName})";
        }

        public XmlElement ToXml(XmlDocument doc, CultureInfo culture)
        {
            XmlElement xmlElement = DescToXml(doc, "Object", Desc);
            xmlElement.SetAttribute("Translator", string.Empty);
            xmlElement.SetAttribute(GH_DescriptionTable.KeyName, GetObjectFullName(this));
            IGH_DocumentObject obj = this.CreateInstance();

            foreach (var item in Diagrams)
            {
                xmlElement.AppendChild(item.ToXml(doc, culture));
            }

            if (obj is GH_Component)
            {
                GH_Component com = (GH_Component)obj;

                XmlElement inputEle = doc.CreateElement("Inputs");
                foreach (var input in com.Params.Input)
                {
                    inputEle.AppendChild(DescToXml(doc, "Param", input));
                }
                xmlElement.AppendChild(inputEle);

                XmlElement outputEle = doc.CreateElement("Outputs");
                foreach (var output in com.Params.Output)
                {
                    outputEle.AppendChild(DescToXml(doc, "Param", output));
                }
                xmlElement.AppendChild(outputEle); ;
            }
            return xmlElement;
        }

        public void Refresh(XmlElement element)
        {
            //Get Proxy Description.
            if (GetObjectFullName(this) != element.GetAttribute(GH_DescriptionTable.KeyName))
                throw new ArgumentException($"Element's {GH_DescriptionTable.KeyName} doesn't fit to this {nameof(GH_LanguageObjectProxy)}");

            string name = element.GetAttribute("Name");
            if (!string.IsNullOrEmpty(name))
                this.Desc.Name = name;

            string nickname = element.GetAttribute("NickName");
            if (!string.IsNullOrEmpty(nickname))
                this.Desc.NickName = nickname;

            string desc = element.GetAttribute("Description");
            if (!string.IsNullOrEmpty(desc))
                this.Desc.Description = desc;

            this.Translator = element.GetAttribute("Translator");
            if (!string.IsNullOrEmpty(Translator))
                this.Desc.Description += "\n----" + Translator;

            if (GH_DescriptionTable.CategoryDict.ContainsKey(this.EnglishCate))
                this.Desc.Category = GH_DescriptionTable.CategoryDict[this.EnglishCate];

            if (GH_DescriptionTable.SubcateDictionary.ContainsKey(this.EnglishCate) && GH_DescriptionTable.SubcateDictionary[this.EnglishCate].ContainsKey(this.EnglishSub))
                this.Desc.Category = GH_DescriptionTable.SubcateDictionary[this.EnglishCate][this.EnglishSub];

            Diagrams.Clear();

            foreach (var param in element.ChildNodes)
            {
                var paramEle = (XmlElement)param;
                if (paramEle.Name == nameof(DiagramForProxy))
                {
                    DiagramForProxy diagram = new DiagramForProxy();
                    diagram.Refresh(paramEle);
                    Diagrams.Add(diagram);
                }
                else if (paramEle.Name == "Inputs")
                {
                    List<string[]> inputsDesc = new List<string[]>();
                    foreach (var input in paramEle.ChildNodes)
                    {
                        var inputEle = (XmlElement)input;
                        string[] inputDesc = new string[] { inputEle.GetAttribute("Name"), inputEle.GetAttribute("NickName"), inputEle.GetAttribute("Description") };
                        inputsDesc.Add(inputDesc);
                    }
                }
                else if (paramEle.Name == "Outputs")
                {
                    List<string[]> outputsDesc = new List<string[]>();
                    foreach (var output in paramEle.ChildNodes)
                    {
                        var outputEle = (XmlElement)output;
                        string[] inputDesc = new string[] { outputEle.GetAttribute("Name"), outputEle.GetAttribute("NickName"), outputEle.GetAttribute("Description") };
                        outputsDesc.Add(inputDesc);
                    }
                }
            }
        }

        private XmlElement DescToXml(XmlDocument doc, string xmlElementName, IGH_InstanceDescription toXMlObject)
        {
            XmlElement xmlElement = doc.CreateElement(xmlElementName);
            xmlElement.SetAttribute(nameof(toXMlObject.Name), toXMlObject.Name);
            xmlElement.SetAttribute(nameof(toXMlObject.NickName), toXMlObject.NickName);
            xmlElement.SetAttribute(nameof(toXMlObject.Description), toXMlObject.Description);
            return xmlElement;
        }

        internal static string GetObjectFullName(IGH_ObjectProxy proxy)
        {
            switch (proxy.Kind)
            {
                case GH_ObjectType.CompiledObject:
                    return proxy.Type.FullName;
                case GH_ObjectType.UserObject:
                    return proxy.Location.Split('\\').Last();
                default:
                    return string.Empty;
            }
        }
    }
}
