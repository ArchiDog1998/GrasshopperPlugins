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
        private static string KeyName => "ObjectFullName";

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
                _cultureInfo = value;
                ChangeLanguage(value);
            }
        }
        #endregion


        #region XML IO
        //private static List<GH_ObjectDescription> ChangeLanguage(CultureInfo info, out List<GH_CategoryDescription> cateDest)
        //{
        //    string pathWithCulture = Path + info.Name + '\\';
        //    string[] allXml = Directory.GetFiles(pathWithCulture, "*.xml");

        //    cateDest = new List<GH_CategoryDescription>();
        //    List<GH_ObjectDescription> outLt = new List<GH_ObjectDescription>();

        //    foreach (var xml in allXml)
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xml);

        //        string recordHash = ((XmlElement)doc.ChildNodes[0]).GetAttribute("HASH");
        //        string calcuHash = UnsafeHelper.HashString(xml.Split('\\').Last());
        //        if (recordHash != calcuHash) continue;


        //        if (xml.Contains("_Category.xml"))
        //        {
        //            foreach (var cateElement in doc.ChildNodes[0].ChildNodes)
        //            {
        //                cateDest.Add(new GH_CategoryDescription((XmlElement)cateElement));
        //            }
        //        }
        //        else
        //        {
        //            XmlElement element = (XmlElement)doc.ChildNodes[0];
        //            foreach (var obj in element.ChildNodes)
        //            {
        //                outLt.Add(new GH_ObjectDescription((XmlElement)obj));
        //            }
        //        }
        //    }

        //    return outLt;
        //}

        private static void ChangeLanguage(CultureInfo info)
        {
        }

        #endregion


        private static string GetObjectFullName(IGH_ObjectProxy proxy)
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
            XmlElement xmlElement = doc.CreateElement("SubCategory");
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
