/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Grasshopper;
using Grasshopper.Kernel;
using Microsoft.VisualBasic.CompilerServices;
using Orthoptera;
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

        public string Translator { get; private set; } = string.Empty;

        public List<DiagramForProxy> Diagrams { get; } = new List<DiagramForProxy>();

        public string EnglishCate { get; }
        public string EnglishSub { get; }
        public string EnglishName { get; }
        public string EnglishNick { get; }
        public string EnglishDesc { get; }
        private List<string[]> EnglishInputParams { get; } = new List<string[]>();
        private List<string[]> EnglishOutputParams { get; } = new List<string[]>();

        public string TranslatedName { get; private set; }
        public string TranslatedNick { get; private set; }
        public string TranslatedDesc { get; private set; }

        private List<string[]> TranslatedInputParams { get; } = new List<string[]>();
        private List<string[]> TranslatedOutputParams { get; } = new List<string[]>();

        public string ClassifyKey { get; }

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
            this.Desc = new GH_InstanceDescription(proxy.Desc);
            this.Exposure = proxy.Exposure;

            if(proxy.Kind == GH_ObjectType.CompiledObject)
            {
                foreach (GH_AssemblyInfo lib in Grasshopper.Instances.ComponentServer.Libraries)
                {
                    if (lib.Assembly == proxy.Type.Assembly)
                    {
                        this.ClassifyKey = lib.Name + "_" + lib.Version;
                        break;
                    }
                }
            }
            else if(proxy.Kind == GH_ObjectType.UserObject)
            {
                this.ClassifyKey = proxy.Desc.Category;
            }

            this.EnglishCate = proxy.Desc.Category;
            this.EnglishSub = proxy.Desc.SubCategory;
            this.EnglishName = proxy.Desc.Name;
            this.EnglishNick = proxy.Desc.NickName;
            this.EnglishDesc = proxy.Desc.Description;

            IGH_DocumentObject obj = proxy.CreateInstance();
            if (obj is GH_Component)
            {
                GH_Component com = (GH_Component)obj;

                foreach (var input in com.Params.Input)
                {
                    EnglishInputParams.Add(new string[] { input.Name, input.NickName, input.Description });
                }
                foreach (var output in com.Params.Output)
                {
                    EnglishOutputParams.Add(new string[] { output.Name, output.NickName, output.Description });
                }
            }
        }

        #region Create and Translate DocumentObject/
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
                        Translate(ref obj);
                        return obj;

                    case GH_ObjectType.UserObject:
                        GH_UserObject gH_UserObject = new GH_UserObject();
                        gH_UserObject.Path = Location;
                        gH_UserObject.ReadFromFile();

                        IGH_DocumentObject user = gH_UserObject.InstantiateObject();
                        Translate(ref user);
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

        private void Translate(ref IGH_DocumentObject obj)
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
                bool isTranslateParamNameAndNick = this.Kind == GH_ObjectType.CompiledObject;
                GH_Component com = obj as GH_Component;
                if(com.Params.Input.Count == TranslatedInputParams.Count)
                {
                    for (int i = 0; i < com.Params.Input.Count; i++)
                    {
                        com.Params.Input[i].Description = EnglishInputParams[i][2] + "\n" + TranslatedInputParams[i][2];
                        if (isTranslateParamNameAndNick)
                        {
                            com.Params.Input[i].Name = EnglishInputParams[i][0] + "(" + TranslatedInputParams[i][0] + ")";
                            com.Params.Input[i].NickName = EnglishInputParams[i][1] + "(" + TranslatedInputParams[i][1] + ")";
                        }
                    }
                }
                else
                {
                    com.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"XML language file's record has a different count of inputs with this components'. Please check it.");
                }

                if(com.Params.Output.Count == TranslatedOutputParams.Count)
                {
                    for (int i = 0; i < com.Params.Output.Count; i++)
                    {
                        com.Params.Output[i].Description = EnglishOutputParams[i][2] + "\n" + TranslatedOutputParams[i][2];
                        if (isTranslateParamNameAndNick)
                        {
                            com.Params.Output[i].Name = EnglishOutputParams[i][0] + "(" + TranslatedOutputParams[i][0] + ")";
                            com.Params.Output[i].NickName = EnglishOutputParams[i][1] + "(" + TranslatedOutputParams[i][1] + ")";
                        }
                    }
                }
                else
                {
                    com.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"XML language file's record has a different count of outputs with this components'. Please check it.");
                }
            }
        }
        #endregion


        public IGH_ObjectProxy DuplicateProxy() => new GH_LanguageObjectProxy(this);


        public void Refresh(XmlElement element)
        {
            //Get Proxy Description.
            if (GetObjectFullName(this) != element.GetAttribute(GH_DescriptionTable.KeyName))
                throw new ArgumentException($"Element's {GH_DescriptionTable.KeyName} doesn't fit to this {nameof(GH_LanguageObjectProxy)}");

            this.TranslatedName = element.GetAttribute("Name");
            if (!string.IsNullOrEmpty(TranslatedName))
                this.Desc.Name = this.EnglishName +"\n" + TranslatedName;

            this.TranslatedNick = element.GetAttribute("NickName");
            if (!string.IsNullOrEmpty(TranslatedNick))
                this.Desc.NickName =this.EnglishNick + "\n" + TranslatedNick;

            this.TranslatedDesc = element.GetAttribute("Description");
            if (!string.IsNullOrEmpty(TranslatedDesc))
                this.Desc.Description = this.EnglishDesc + "\n" + TranslatedDesc;

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
                    TranslatedInputParams.Clear();
                    foreach (var input in paramEle.ChildNodes)
                    {
                        var inputEle = (XmlElement)input;
                        string[] inputDesc = new string[] 
                        {
                            inputEle.GetAttribute("Name"), 
                            inputEle.GetAttribute("NickName"), 
                            inputEle.GetAttribute("Description"),
                        };
                        TranslatedInputParams.Add(inputDesc);
                    }
                }
                else if (paramEle.Name == "Outputs")
                {
                    TranslatedOutputParams.Clear();
                    foreach (var output in paramEle.ChildNodes)
                    {
                        var outputEle = (XmlElement)output;
                        string[] inputDesc = new string[] 
                        {
                            outputEle.GetAttribute("Name"), 
                            outputEle.GetAttribute("NickName"), 
                            outputEle.GetAttribute("Description"),
                        };
                        TranslatedOutputParams.Add(inputDesc);
                    }
                }
            }
        }

        public void Save()
        {
            CultureInfo info = GH_DescriptionTable.Language;
            string path = GH_DescriptionTable.Path + info.Name + '\\';
            string fileName = $"{info.Name}_ObjectSet_{ClassifyKey}.xml";
            string location = path + fileName;

            Directory.CreateDirectory(path);
            

            bool isHaveXmlAndValid = false;
            if(File.Exists(location))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(location);

                string recordHash = ((XmlElement)doc.ChildNodes[0]).GetAttribute("HASH");
                string calcuHash = UnsafeHelper.HashString(fileName);
                if (recordHash == calcuHash)
                {
                    isHaveXmlAndValid = true;
                    XmlElement element = (XmlElement)doc.ChildNodes[0];

                    bool isFindEle = false;
                    foreach (var obj in element.ChildNodes)
                    {
                        var objEle = (XmlElement)obj;

                        //Get Proxy Description.
                        string objFullName = objEle.GetAttribute(GH_DescriptionTable.KeyName);

                        if (objFullName == GetObjectFullName(this))
                        {
                            //Change Reference.
                            objEle = ToXml(doc, info);
                            isFindEle = true;
                            break;
                        }
                    }

                    //Append a new Language Item.
                    if (!isFindEle)
                    {
                        doc.ChildNodes[0].AppendChild(ToXml(doc, info));
                    }

                    doc.Save(location);
                }
            }

            if (!isHaveXmlAndValid)
            {
                //Create a brand new XmlDocument and save it.
                GH_DescriptionTable.CreateXmlDocumentAndSave(path, fileName, (doc) =>
                {
                    XmlElement xmlElement = doc.CreateElement("DocumentObjects");
                    xmlElement.AppendChild(ToXml(doc, info));
                    return xmlElement;
                });
            }
        }

        public XmlElement ToXml(XmlDocument doc, CultureInfo culture)
        {
            XmlElement xmlElement = DescToXml(doc, "Object", this.TranslatedName, this.TranslatedNick, this.TranslatedDesc);
            xmlElement.SetAttribute("Translator", this.Translator);
            xmlElement.SetAttribute(GH_DescriptionTable.KeyName, GetObjectFullName(this));

            foreach (var item in Diagrams)
            {
                xmlElement.AppendChild(item.ToXml(doc, culture));
            }

            XmlElement inputEle = doc.CreateElement("Inputs");
            foreach (var input in TranslatedInputParams)
            {
                inputEle.AppendChild(DescToXml(doc, "Param", input[0], input[1], input[2]));
            }
            xmlElement.AppendChild(inputEle);

            XmlElement outputEle = doc.CreateElement("Outputs");
            foreach (var output in TranslatedOutputParams)
            {
                outputEle.AppendChild(DescToXml(doc, "Param", output[0], output[1], output[2]));
            }
            xmlElement.AppendChild(outputEle);


            return xmlElement;
        }
        private XmlElement DescToXml(XmlDocument doc, string xmlElementName, string name, string nickName, string description)
        {
            XmlElement xmlElement = doc.CreateElement(xmlElementName);
            xmlElement.SetAttribute("Name", name);
            xmlElement.SetAttribute("NickName", nickName);
            xmlElement.SetAttribute("Description", description);
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
