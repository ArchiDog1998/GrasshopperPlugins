/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using Grasshopper.Kernel.Special;
using System.Drawing;
using System.Windows.Forms;

namespace Orthoptera.Language
{
    public static class GH_DescriptionTable
    {
        private static string Path => Grasshopper.Folders.AppDataFolder + "Language\\";
        private static string KeyName => "ObjectFullName";

        private static readonly FieldInfo _cateIcon = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categoryIcons")).First();
        private static readonly FieldInfo _cateShort = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categoryShort")).First();
        private static readonly FieldInfo _cateSymbol = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categorySymbol")).First();
        private static readonly FieldInfo _proxies = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_proxies")).First();

        private static readonly  SortedList<string, Bitmap> _oldCateIcons = (SortedList<string, Bitmap>)_cateIcon.GetValue(Instances.ComponentServer);
        private static readonly SortedList<string, string> _oldCateShort = (SortedList<string, string>)_cateShort.GetValue(Instances.ComponentServer);
        private static readonly SortedList<string, string> _oldCateSymbol = (SortedList<string, string>)_cateSymbol.GetValue(Instances.ComponentServer);
        private static readonly SortedList<Guid, IGH_ObjectProxy> _oldProxies = (SortedList<Guid, IGH_ObjectProxy>)_proxies.GetValue(Instances.ComponentServer);

        #region Culture Property
        private static CultureInfo _cultureInfo = new CultureInfo(1033);
        public static CultureInfo Culture
        {
            get 
            { 
                return _cultureInfo; 
            }
            set 
            {
                if(value.Name != _cultureInfo.Name)
                {
                    _cultureInfo = value;
                    ChangeLanguage(value);
                }
            }
        }
        #endregion


        #region XML IO
        private static void ChangeLanguage(CultureInfo info)
        {

            string path = Path + info.Name + '\\';
            string[] allXml = Directory.GetFiles(path, "*.xml");

            Dictionary<string, string[]> categoryDict = new Dictionary<string, string[]>();
            Dictionary<string, Dictionary<string, string>> subcateDict = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string[]> objectDescDict = new Dictionary<string, string[]>();
            Dictionary<string, List<string[]>> componentInputsDict = new Dictionary<string, List<string[]>>();
            Dictionary<string, List<string[]>> componentOutputsDict = new Dictionary<string, List<string[]>>();

            //Read XML to Dictionary.
            foreach (string xml in allXml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xml);

                string recordHash = ((XmlElement)doc.ChildNodes[0]).GetAttribute("HASH");
                string calcuHash = UnsafeHelper.HashString(xml.Split('\\').Last());
                if (recordHash != calcuHash) continue;

                if (xml.Contains($"_{info.Name}_Category.xml"))
                {
                    foreach (var cateElement in doc.ChildNodes[0].ChildNodes)
                    {
                        var ele = (XmlElement)cateElement;

                        //Get Category infos.
                        string cateFullName = ele.GetAttribute(KeyName);
                        string[] cateInfo = new string[] { ele.GetAttribute("Name"), ele.GetAttribute("ShortName"), ele.GetAttribute("SymbolName") };
                        categoryDict[cateFullName] = cateInfo;


                        Dictionary<string, string> subDict = new Dictionary<string, string>();
                        foreach (var subElement in ele.ChildNodes)
                        {
                            var subEle = (XmlElement)subElement;

                            //Get Subcategory infos.
                            string subFullName = subEle.GetAttribute(KeyName);
                            string subName = subEle.GetAttribute("Name");
                            subDict[subFullName] = subName;
                        }
                        subcateDict[cateFullName] = subDict;
                    }
                }
                else
                {
                    XmlElement element = (XmlElement)doc.ChildNodes[0];
                    foreach (var obj in element.ChildNodes)
                    {
                        var objEle = (XmlElement)obj;

                        //Get Proxy Description.
                        string objFullName = objEle.GetAttribute(KeyName);
                        string[] objDesc = new string[] { objEle.GetAttribute("Name"), objEle.GetAttribute("NickName"), objEle.GetAttribute("Description")};
                        objectDescDict[objFullName] = objDesc;

                        foreach (var param in objEle.ChildNodes)
                        {
                            var paramEle = (XmlElement)param;
                            if( paramEle.Name == "Inputs")
                            {
                                List<string[]> inputsDesc = new List<string[]>();
                                foreach (var input in paramEle.ChildNodes)
                                {
                                    var inputEle = (XmlElement)input;
                                    string[] inputDesc = new string[] { inputEle.GetAttribute("Name"), inputEle.GetAttribute("NickName"), inputEle.GetAttribute("Description") };
                                    inputsDesc.Add(inputDesc);
                                }
                                componentInputsDict[objFullName] = inputsDesc;
                            }
                            else if(paramEle.Name == "Outputs")
                            {
                                List<string[]> outputsDesc = new List<string[]>();
                                foreach (var output in paramEle.ChildNodes)
                                {
                                    var outputEle = (XmlElement)output;
                                    string[] inputDesc = new string[] { outputEle.GetAttribute("Name"), outputEle.GetAttribute("NickName"), outputEle.GetAttribute("Description") };
                                    outputsDesc.Add(inputDesc);
                                }
                                componentOutputsDict[objFullName] = outputsDesc;
                            }
                        }
                    }
                }
            }

            ChangeCategoriesLanguage(categoryDict);
            ChangeProxiesLanguage(categoryDict, subcateDict, objectDescDict, componentInputsDict, componentOutputsDict);

            //Update Ribbon.
            ((Grasshopper.GUI.Ribbon.GH_Ribbon)Instances.DocumentEditor.Controls[3]).PopulateRibbon();
        }

        private static void ChangeProxiesLanguage(Dictionary<string, string[]> categoryDict, Dictionary<string, Dictionary<string, string>> cateSubCateDict,
            Dictionary<string, string[]> ObjDescDict, Dictionary<string, List<string[]>> componentInputsDict, Dictionary<string, List<string[]>> componentOutputsDict)
        {

            var newProxies = new SortedList<Guid, IGH_ObjectProxy>();

            foreach (var oldProxy in _oldProxies)
            {
                //Get Category and Subcategory Translate.

                string category = string.Empty;
                string subCategory = string.Empty;
                if (oldProxy.Value.Desc.HasCategory)
                {
                    category = oldProxy.Value.Desc.Category;

                    if (categoryDict.ContainsKey(oldProxy.Value.Desc.Category))
                        category = categoryDict[oldProxy.Value.Desc.Category][0];

                    if (oldProxy.Value.Desc.HasSubCategory)
                    {
                        subCategory = oldProxy.Value.Desc.SubCategory;
                        if (cateSubCateDict.ContainsKey(oldProxy.Value.Desc.Category))
                        {
                            var subCateDict = cateSubCateDict[oldProxy.Value.Desc.Category];
                            if (subCateDict.ContainsKey(oldProxy.Value.Desc.SubCategory))
                            {
                                subCategory = subCateDict[oldProxy.Value.Desc.SubCategory];
                            }
                        }
                    }
                }

                string objFullName = GetObjectFullName(oldProxy.Value);

                //Get Base Information.
                string[] descBase = new string[] { oldProxy.Value.Desc.Name, oldProxy.Value.Desc.NickName, oldProxy.Value.Desc.Description };
                if (ObjDescDict.ContainsKey(objFullName))
                {
                    descBase = ObjDescDict[objFullName];
                }

                //Set base Infomation
                string[] whole = new string[] { descBase[0], descBase[1], descBase[2], category, subCategory };
                var newProxy = new GH_LanguageObjectProxy(oldProxy.Value, whole);

                //Change Params.
                if (componentInputsDict.ContainsKey(objFullName))
                    newProxy.InputParams = componentInputsDict[objFullName];
                if (componentOutputsDict.ContainsKey(objFullName))
                    newProxy.OutputParams = componentOutputsDict[objFullName];

                //Add to List.
                newProxies[oldProxy.Key] = newProxy;
            }

            _proxies.SetValue(Instances.ComponentServer, newProxies);
        }

        private static void ChangeCategoriesLanguage(Dictionary<string, string[]> newCateInfos)
        {
            var newCateIcons = new SortedList<string, Bitmap>();
            var newCateShort = new SortedList<string, string>();
            var newCateSymbol = new SortedList<string, string>();

            foreach (var oldIcon in _oldCateIcons)
            {
                if (newCateInfos.ContainsKey(oldIcon.Key))
                {
                    newCateIcons[newCateInfos[oldIcon.Key][0]] = oldIcon.Value;
                }
            }
            foreach (var oldShort in _oldCateShort)
            {
                if (newCateInfos.ContainsKey(oldShort.Key))
                {
                    newCateShort[newCateInfos[oldShort.Key][0]] = newCateInfos[oldShort.Key][1];
                }
            }
            foreach (var oldSymbol in _oldCateSymbol)
            {
                if (newCateInfos.ContainsKey(oldSymbol.Key))
                {
                    newCateSymbol[newCateInfos[oldSymbol.Key][0]] = newCateInfos[oldSymbol.Key][2];
                }
            }

            _cateIcon.SetValue(Instances.ComponentServer, newCateIcons);
            _cateShort.SetValue(Instances.ComponentServer, newCateShort);
            _cateSymbol.SetValue(Instances.ComponentServer, newCateSymbol);
        }


        #endregion


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

        #region Write XML
        public static void WriteXml(CultureInfo info)
        {
            CultureInfo oldInfo = null;
            if (Culture != new CultureInfo(1033))
            {
                oldInfo = Culture;
                Culture = new CultureInfo(1033);
            }

            Directory.CreateDirectory($"{Path}\\{info.Name}");

            Dictionary<string, SortedSet<string>> cateSubSets = new Dictionary<string, SortedSet<string>>();

            //List<CateSubSet> cateSubSets = new List<CateSubSet>();
            List<IGH_ObjectProxy> compiledProxies = new List<IGH_ObjectProxy>();
            List<IGH_ObjectProxy> userobjectProxies = new List<IGH_ObjectProxy>();
            foreach (var proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if (proxy.Kind == GH_ObjectType.CompiledObject)
                {
                    compiledProxies.Add(proxy);
                }
                else if (proxy.Kind == GH_ObjectType.UserObject)
                {
                    userobjectProxies.Add(proxy);
                }

                //Category Add.
                if (proxy.Desc.HasCategory && proxy.Desc.HasSubCategory)
                {
                    if (cateSubSets.ContainsKey(proxy.Desc.Category))
                    {
                        cateSubSets[proxy.Desc.Category].Add(proxy.Desc.SubCategory);
                    }
                    else
                    {
                        cateSubSets[proxy.Desc.Category] = new SortedSet<string>() { proxy.Desc.SubCategory };
                    }
                }
            }


            //Group the CompiledProxies with their libraries.
            foreach (var group in compiledProxies.GroupBy((proxy) => {
                foreach (GH_AssemblyInfo lib in Grasshopper.Instances.ComponentServer.Libraries)
                {
                    if (lib.Assembly == proxy.Type.Assembly)
                    {
                        return lib.Name + "_" + lib.Version;
                    }
                }
                return "NullLibrary";
            }))
            {
                AssemblyWriteXml(info, group.ToList(), group.Key);
            }

            //Group the UserProxies with their categories.
            foreach (var group in userobjectProxies.GroupBy((proxy) => proxy.Desc.Category))
            {
                AssemblyWriteXml(info, group.ToList(), group.Key);
            }

            CategoryWriteXml(info, cateSubSets);

            if (oldInfo != null)
            {
                Culture = oldInfo;
            }
        }

        private static void AssemblyWriteXml(CultureInfo culture, List<IGH_ObjectProxy> proxies, string assemblyName)
        {
            string fileName = $"{culture.Name}_{assemblyName}.xml";
            WriteXml(culture, fileName, (doc) => AssemblyToXml(doc, proxies));
        }

        private static void CategoryWriteXml(CultureInfo culture, Dictionary<string, SortedSet<string>> cateSubSet)
        {
            string fileName = $"_{culture.Name}_Category.xml";
            WriteXml(culture, fileName, (doc) => CategoriesToXml(doc, cateSubSet));
        }

        private static void WriteXml(CultureInfo culture, string fileName, Func<XmlDocument, XmlElement> getElement)
        {
            string location = $"{Path}\\{culture.Name}\\{fileName}";
            string sha = UnsafeHelper.HashString(fileName);

            XmlDocument doc = new XmlDocument();
            XmlElement element = getElement(doc);
            element.SetAttribute("HASH", sha);

            doc.AppendChild(element);
            doc.Save(location);
        }
        #endregion

        #region ToXMl
        #region CategoryToXml
        private static XmlElement CategoriesToXml(XmlDocument doc, Dictionary<string, SortedSet<string>> cateSubSet)
        {
            
            SortedList<string, string> shortCategories = (SortedList<string, string>)typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categoryShort")).First().GetValue(Grasshopper.Instances.ComponentServer);
            SortedList<string, string> symbolCategories = (SortedList<string, string>)typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categorySymbol")).First().GetValue(Grasshopper.Instances.ComponentServer);

            XmlElement xmlElement = doc.CreateElement("Categories");
            xmlElement.SetAttribute("Translator", string.Empty);
            foreach (var pair in cateSubSet)
            {
                string cate = pair.Key;

                string shortCate = "";
                if (shortCategories.ContainsKey(cate))
                    shortCate = shortCategories[cate];

                string symbolCate = "";
                if (symbolCategories.ContainsKey(cate))
                    symbolCate = symbolCategories[cate];

                xmlElement.AppendChild(CategoryToXml(doc, cate, shortCate, symbolCate, pair.Value));
            }
            return xmlElement;
        }

        private static XmlElement CategoryToXml(XmlDocument doc, string category, string shortName, string symbol, SortedSet<string> subCategories)
        {
            XmlElement xmlElement = doc.CreateElement("Category");
            xmlElement.SetAttribute("Name", category);
            xmlElement.SetAttribute("ShortName", shortName);
            xmlElement.SetAttribute("SymbolName", symbol);
            xmlElement.SetAttribute(KeyName, category);
            foreach (var subCategory in subCategories)
            {
                xmlElement.AppendChild(SubCateToXml(doc, subCategory));
            }
            return xmlElement;
        }
        private static XmlElement SubCateToXml(XmlDocument doc, string name)
        {
            XmlElement xmlElement = doc.CreateElement("SubCategory");
            xmlElement.SetAttribute("Name", name);
            xmlElement.SetAttribute(KeyName, name);
            return xmlElement;
        }
        #endregion

        #region ObjectProxyToXml
        private static XmlElement AssemblyToXml(XmlDocument doc, List<IGH_ObjectProxy> proxies)
        {
            XmlElement xmlElement = doc.CreateElement("DocumentObjects");
            foreach (var proxy in proxies)
            {
                xmlElement.AppendChild(ProxyToXml(doc, proxy));
            }
            return xmlElement;
        }

        private static XmlElement ProxyToXml(XmlDocument doc, IGH_ObjectProxy proxy)
        {
            XmlElement xmlElement = DescToXml(doc, "Object", proxy.Desc);
            xmlElement.SetAttribute("Translator", string.Empty);
            xmlElement.SetAttribute(KeyName, GetObjectFullName(proxy));

            IGH_DocumentObject obj = proxy.CreateInstance();
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

        private static XmlElement DescToXml(XmlDocument doc, string xmlElementName, IGH_InstanceDescription toXMlObject)
        {
            XmlElement xmlElement = doc.CreateElement(xmlElementName);
            xmlElement.SetAttribute(nameof(toXMlObject.Name), toXMlObject.Name);
            xmlElement.SetAttribute(nameof(toXMlObject.NickName), toXMlObject.NickName);
            xmlElement.SetAttribute(nameof(toXMlObject.Description), toXMlObject.Description);
            return xmlElement;
        }
        #endregion
        #endregion
    }


}
