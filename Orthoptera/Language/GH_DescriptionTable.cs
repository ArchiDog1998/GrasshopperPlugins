/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

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

namespace Orthoptera.Language
{
    public static class GH_DescriptionTable
    {
        private static string Path => Grasshopper.Folders.AppDataFolder + "Language\\";
        private static List<GH_ObjectDescription> ObjDescSet { get; set; } = new List<GH_ObjectDescription>();
        private static List<GH_ObjectDescription> EnglishDescSet { get; } = ChangeLanguage(new CultureInfo(1033), out _);

        private static List<GH_CategoryDescription> CateDescSet { get; set; } = new List<GH_CategoryDescription>();

        #region Culture Property
        private static CultureInfo _cultureInfo;
        public static CultureInfo Culture
        {
            get 
            { 
                return _cultureInfo; 
            }
            set 
            { 
                _cultureInfo = value;
                Clear();
                ChangeLanguage(value);
            }
        }
        #endregion

        #region Translate
        public static void StartTranslate()
        {
            Grasshopper.Instances.ActiveCanvas.Document_ObjectsAdded += LangaugeChange;
        }

        private static void LangaugeChange(GH_Document sender, GH_DocObjectEventArgs e)
        {
            foreach (var obj in e.Objects)
            {
                string type = obj.GetType().FullName;
                if (type.Contains("GH_Cluster")|| type.Contains("ScriptComponents."))
                {
                    bool isTranslate = false;

                    var thisDesc = new GH_ObjectDescription(obj);
                    foreach (var item in EnglishDescSet)
                    {
                        if (item.Equals(thisDesc))
                        {
                            TranslateIt(obj, item.ObjectFullName, obj is GH_Cluster);
                            isTranslate = true;
                            break;
                        }
                    }
                    if (isTranslate) continue;
                }

                if (type == "Component_CSNET_Script" || type == "ZuiPythonComponent" || type == "Component_VBNET_Script"
                    || type == "Component_Expression" || type == "Component_Evaluate")
                {

                }
            }
        }

        public static IGH_DocumentObject TranslateIt(IGH_DocumentObject obj, string objectFullName, bool isTranslateIOName)
        {
            GH_ObjectDescription desc = null;
            foreach (var item in ObjDescSet)
            {
                if(item.ObjectFullName == objectFullName)
                {
                    desc = item;
                    break;
                }
            }
            if (desc == null) return obj;

            obj.Name = desc.Name;
            obj.NickName = desc.NickName;
            obj.Description = desc.Description + $"\n{nameof(desc.Translator)}: " + desc.Translator;

            if(obj is GH_Component)
            {
                GH_Component com = (GH_Component)obj;
                for (int i = 0; i < com.Params.Input.Count; i++)
                {
                    if (isTranslateIOName)
                    {
                        com.Params.Input[i].Name = desc.InputParamDescription[i].Name;
                        com.Params.Input[i].NickName = desc.InputParamDescription[i].NickName;
                    }
                    com.Params.Input[i].Description = desc.InputParamDescription[i].Description;
                }
                for (int i = 0; i < com.Params.Output.Count; i++)
                {
                    if (isTranslateIOName)
                    {
                        com.Params.Output[i].Name = desc.OutputParamDescription[i].Name;
                        com.Params.Output[i].NickName = desc.OutputParamDescription[i].NickName;
                    }
                    com.Params.Output[i].Description = desc.OutputParamDescription[i].Description;
                }
            }
            return obj;
        }
        #endregion

        private static void Clear()
        {
            ObjDescSet.Clear();
            CateDescSet.Clear();
        }

        #region XML IO
        private static List<GH_ObjectDescription> ChangeLanguage(CultureInfo info, out List<GH_CategoryDescription> cateDest)
        {
            string pathWithCulture = Path + info.Name + '\\';
            string[] allXml = Directory.GetFiles(pathWithCulture, "*.xml");

            cateDest = new List<GH_CategoryDescription>();
            List<GH_ObjectDescription> outLt = new List<GH_ObjectDescription>();

            foreach (var xml in allXml)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xml);

                string recordHash = ((XmlElement)doc.ChildNodes[0]).GetAttribute("HASH");
                string calcuHash = UnsafeHelper.HashString(xml.Split('\\').Last());
                if (recordHash != calcuHash) continue;


                if (xml.Contains("_Category.xml"))
                {
                    foreach (var cateElement in doc.ChildNodes[0].ChildNodes)
                    {
                        cateDest.Add(new GH_CategoryDescription((XmlElement)cateElement));
                    }
                }
                else
                {
                    XmlElement element = (XmlElement)doc.ChildNodes[0];
                    foreach (var obj in element.ChildNodes)
                    {
                        outLt.Add(new GH_ObjectDescription((XmlElement)obj));
                    }
                }
            }

            return outLt;
        }

        private static void ChangeLanguage(CultureInfo info)
        {
            List<GH_CategoryDescription> cateDest = new List<GH_CategoryDescription>();
            ObjDescSet = ChangeLanguage(info, out cateDest);
            CateDescSet = cateDest;
        }

        #region WriteXml to translate.
        public static void WriteXml(CultureInfo info)
        {
            string pathWithCulture = Path + info.Name + '\\';
            Directory.CreateDirectory(pathWithCulture);

            //Write XML.
            WriteCategories(pathWithCulture, info, WriteProxies(pathWithCulture, info));
        }

        private static void WriteCategories(string pathWithCulture, CultureInfo info, List<CateSubSet> cateSubSet)
        {
            SortedList<string, string> shortCategories = (SortedList<string, string>)GH_CategoryDescription.ShortField.GetValue(Grasshopper.Instances.ComponentServer);
            SortedList<string, string> symbolCategories = (SortedList<string, string>)GH_CategoryDescription.SymbolField.GetValue(Grasshopper.Instances.ComponentServer);

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Categories");

            foreach (var subSet in cateSubSet.GroupBy((dictPair) => dictPair.Category))
            {
                string cate = subSet.Key;

                string shortCate = "";
                if(shortCategories.ContainsKey(cate))
                    shortCate = shortCategories[cate];

                string symbolCate = "";
                if (symbolCategories.ContainsKey(cate))
                    symbolCate = symbolCategories[cate];

                List<string> subCates = subSet.Select((pair) => pair.SubCategory).ToList();
                subCates.Sort();
                root.AppendChild(new GH_CategoryDescription(cate, shortCate, symbolCate, subCates).ToXml(doc));
            }

            string fileName = $"_{info.Name}_Category.xml";
            string sha = UnsafeHelper.HashString(fileName);
            root.SetAttribute("HASH", sha);
            doc.AppendChild(root);
            doc.Save(pathWithCulture + fileName);
        }

        private static List<CateSubSet> WriteProxies(string pathWithCulture, CultureInfo info)
        {
            List<CateSubSet> cateSubSets = new List<CateSubSet>();
            List<IGH_ObjectProxy> compiledProxies = new List<IGH_ObjectProxy>();
            List<IGH_ObjectProxy> userobjectProxies = new List<IGH_ObjectProxy>();
            foreach (var proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                bool isAdd = true;
                if (proxy.Kind == GH_ObjectType.CompiledObject)
                {
                    compiledProxies.Add(proxy);
                }
                else if (proxy.Kind == GH_ObjectType.UserObject)
                {
                    userobjectProxies.Add(proxy);
                }
                if (proxy.Desc.HasCategory && proxy.Desc.HasSubCategory)
                {
                    CateSubSet cateSubSet = new CateSubSet(proxy.Desc.Category, proxy.Desc.SubCategory);
                    foreach (var item in cateSubSets)
                    {
                        if (item.Equals(cateSubSet))
                        {
                            isAdd = false;
                            break;
                        }
                    }
                    if (isAdd)
                    {
                        cateSubSets.Add(cateSubSet);
                    }
                }
            }

            List<GH_ObjectDescriptionList> descSet = new List<GH_ObjectDescriptionList>();

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
                new GH_ObjectDescriptionList(group.ToList()).WriteXml($"{pathWithCulture}{info.Name}_{group.Key}.xml");
            }

            //Group the UserProxies with their categories.
            foreach (var group in userobjectProxies.GroupBy((proxy) => proxy.Desc.Category))
            {
                new GH_ObjectDescriptionList(group.ToList()).WriteXml($"{pathWithCulture}{info.Name}_{group.Key}.xml");
            }

            return cateSubSets;
        }
        #endregion
        #endregion

        private struct CateSubSet
        {
            internal string Category { get; }
            internal string SubCategory { get; }

            internal CateSubSet(string cate, string subCate)
            {
                this.Category = cate;
                this.SubCategory = subCate;
            }

            internal bool Equals(CateSubSet other) => this.Category == other.Category && this.SubCategory == other.SubCategory;
        }
    }


}
