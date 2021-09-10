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
using Grasshopper.GUI.Ribbon;
using Orthoptera;

namespace OrthopteraUI.Language
{
    public static class GH_DescriptionTable
    {
        public static string Path => Folders.AppDataFolder + "Language\\";
        public static CultureInfo[] Languages => Directory.GetDirectories(Path).Select((fullPath) => new CultureInfo(fullPath.Split('\\').Last())).ToArray();
        internal static string KeyName => "ObjectFullName";

        #region Reflection
        private static readonly FieldInfo _cateIcon = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categoryIcons")).First();
        private static readonly FieldInfo _cateShort = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categoryShort")).First();
        private static readonly FieldInfo _cateSymbol = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categorySymbol")).First();
        private static readonly FieldInfo _proxies = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_proxies")).First();
        #endregion

        #region OldInfomations
        private static readonly SortedList<string, Bitmap> _oldCateIcons = (SortedList<string, Bitmap>)_cateIcon.GetValue(Instances.ComponentServer);
        private static readonly SortedList<string, string> _oldCateShort = (SortedList<string, string>)_cateShort.GetValue(Instances.ComponentServer);
        private static readonly SortedList<string, string> _oldCateSymbol = (SortedList<string, string>)_cateSymbol.GetValue(Instances.ComponentServer);
        //private static readonly SortedList<Guid, IGH_ObjectProxy> _oldProxies = (SortedList<Guid, IGH_ObjectProxy>)_proxies.GetValue(Instances.ComponentServer);
        private static readonly Dictionary<string, SortedSet<string>> _oldCateSubSets = new Dictionary<string, SortedSet<string>>();
        #endregion

        #region RightCategoryInfomations
        public static Dictionary<string, string> CategoryDict { get; private set; } = new Dictionary<string, string>();
        public static Dictionary<string, Dictionary<string, string>> SubcateDictionary { get; private set; } = new Dictionary<string, Dictionary<string, string>>();
        #endregion


        public static bool CanTranslate { get; private set; } = false;

        #region Culture Property
        private static CultureInfo _cultureInfo = new CultureInfo(1033);
        public static CultureInfo Language
        {
            get 
            { 
                return _cultureInfo; 
            }
            set 
            {
                _cultureInfo = value;
                if (CanTranslate)
                    ChangeLanguage(value);
            }
        }
        #endregion

        /// <summary>
        /// this Function need to be used before you translation the whole Grasshopper. Besides, this method only need to be used only once.
        /// </summary>
        public static void EnableTranslation()
        {
            CanTranslate = true;


            SortedList<Guid, IGH_ObjectProxy> proxies = (SortedList<Guid, IGH_ObjectProxy>)_proxies.GetValue(Instances.ComponentServer);
            foreach (var item in proxies)
            {
                proxies[item.Key] = new GH_LanguageObjectProxy(item.Value);


                //Category Add.
                if (item.Value.Desc.HasCategory && item.Value.Desc.HasSubCategory)
                {
                    if (_oldCateSubSets.ContainsKey(item.Value.Desc.Category))
                    {
                        _oldCateSubSets[item.Value.Desc.Category].Add(item.Value.Desc.SubCategory);
                    }
                    else
                    {
                        _oldCateSubSets[item.Value.Desc.Category] = new SortedSet<string>() { item.Value.Desc.SubCategory };
                    }
                }
            }
            _proxies.SetValue(Instances.ComponentServer, proxies);
        }

        #region XML IO

        #region Change(Read) Language
        private static void ChangeLanguage(CultureInfo info)
        {

            string path = Path + info.Name + '\\';
            string[] allXml = Directory.GetFiles(path, "*.xml");

            Dictionary<string, string[]> categoryDict = new Dictionary<string, string[]>();

            SortedList<Guid, IGH_ObjectProxy> proxies = (SortedList<Guid, IGH_ObjectProxy>)_proxies.GetValue(Instances.ComponentServer);

            CategoryDict.Clear();
            SubcateDictionary.Clear();

            //Change the Categories' and Subcategories' languages.
            ReadXmlElement(allXml, info, (xml) => xml.Contains($"_{info.Name}_Category.xml"), (ele) =>
            {
                //Get Category infos.
                string cateFullName = ele.GetAttribute(KeyName);
                string[] cateInfo = new string[] { ele.GetAttribute("Name"), ele.GetAttribute("ShortName"), ele.GetAttribute("SymbolName") };
                categoryDict[cateFullName] = cateInfo;
                CategoryDict[cateFullName] = cateInfo[0];

                Dictionary<string, string> subDict = new Dictionary<string, string>();
                foreach (var subElement in ele.ChildNodes)
                {
                    var subEle = (XmlElement)subElement;

                    //Get Subcategory infos.
                    string subFullName = subEle.GetAttribute(KeyName);
                    string subName = subEle.GetAttribute("Name");
                    subDict[subFullName] = subName;
                }
                SubcateDictionary[cateFullName] = subDict;
            });
            ChangeCategoriesLanguage(categoryDict);


            //Change the Proxy's Language.
            ReadXmlElement(allXml, info, (xml) => xml.Contains($"_{info.Name}_ObjectSet_"), (objEle) =>
            {
                string objFullName = objEle.GetAttribute(KeyName);

                foreach (var proxy in proxies)
                {
                    if (GH_LanguageObjectProxy.GetObjectFullName(proxy.Value) == objFullName)
                    {
                        GH_LanguageObjectProxy langProxy;
                        if (proxy.Value is GH_LanguageObjectProxy)
                        {
                            langProxy = (GH_LanguageObjectProxy)proxy.Value;
                        }
                        else
                        {
                            langProxy = new GH_LanguageObjectProxy(proxy.Value);
                        }
                        langProxy.Refresh(objEle);
                        proxies[proxy.Key] = langProxy;
                        return;
                    }
                }
            });
            _proxies.SetValue(Instances.ComponentServer, proxies);

            //Update Ribbon.
            UpdateGHRibbon(categoryDict);
        }

        private static void ReadXmlElement(string[] location, CultureInfo info, Func<string, bool> isThisXml, Action<XmlElement> doingToXml)
        {
            foreach (string xml in location)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xml);

                string recordHash = ((XmlElement)doc.ChildNodes[0]).GetAttribute("HASH");
                string calcuHash = UnsafeHelper.HashString(xml.Split('\\').Last());
                if (recordHash != calcuHash) continue;

                if (isThisXml(xml))
                {
                    XmlElement element = (XmlElement)doc.ChildNodes[0];
                    foreach (var obj in element.ChildNodes)
                    {
                        XmlElement objEle = (XmlElement)obj;
                        doingToXml(objEle);
                    }
                }
            }
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

        #region UpdateRibbon
        private static void UpdateGHRibbon(Dictionary<string, string[]> categoryDict)
        {
            GH_Ribbon ribbon = (GH_Ribbon)Instances.DocumentEditor.Controls[3];

            MethodInfo EnsureTabInfo = typeof(GH_Ribbon).GetRuntimeMethods().Where((method) => method.Name.Contains("EnsureTab")).First();

            string activeTabName = ribbon.ActiveTabName;
            if (categoryDict.ContainsKey(activeTabName))
                activeTabName = categoryDict[activeTabName][0];

            ribbon.Tabs.Clear();
            List<GH_Layout> list = new List<GH_Layout>();
            list.Add(GetLayout(categoryDict));
            foreach (GH_Layout item in list)
            {
                foreach (GH_LayoutTab tab in item.Tabs)
                {
                    GH_RibbonTab gH_RibbonTab = (GH_RibbonTab)EnsureTabInfo.Invoke(ribbon, new object[] { tab.Name });
                    foreach (GH_LayoutPanel panel in tab.Panels)
                    {
                        GH_RibbonPanel gH_RibbonPanel = gH_RibbonTab.EnsurePanel(panel.Name);
                        foreach (GH_LayoutItem item2 in panel.Items)
                        {
                            if (!gH_RibbonPanel.Contains(item2.Id, item2.Exposure))
                            {
                                IGH_ObjectProxy iGH_ObjectProxy = Instances.ComponentServer.EmitObjectProxy(item2.Id);
                                if (iGH_ObjectProxy != null)
                                {
                                    iGH_ObjectProxy = iGH_ObjectProxy.DuplicateProxy();
                                    iGH_ObjectProxy.Exposure = item2.Exposure;
                                    iGH_ObjectProxy.Desc.Category = tab.Name;
                                    iGH_ObjectProxy.Desc.SubCategory = panel.Name;
                                    gH_RibbonPanel.AddItem(new GH_RibbonItem(iGH_ObjectProxy));
                                }
                            }
                        }
                    }
                }
            }
            ribbon.Tabs[0].Visible = true;
            foreach (GH_RibbonTab tab2 in ribbon.Tabs)
            {
                foreach (GH_RibbonPanel panel2 in tab2.Panels)
                {
                    panel2.Sort();
                }
            }
            ribbon.ActiveTabName = activeTabName;
            ribbon.LayoutRibbon();
            ribbon.Refresh();
        }

        private static GH_Layout GetLayout(Dictionary<string, string[]> categoryDict)
        {
            GH_Layout gH_Layout = new GH_Layout();
            gH_Layout.FilePath = "Default";
            gH_Layout.AddTab(categoryDict["Params"][0]);
            gH_Layout.AddTab(categoryDict["Maths"][0]);
            gH_Layout.AddTab(categoryDict["Sets"][0]);
            gH_Layout.AddTab(categoryDict["Vector"][0]);
            gH_Layout.AddTab(categoryDict["Curve"][0]);
            gH_Layout.AddTab(categoryDict["Surface"][0]);
            gH_Layout.AddTab(categoryDict["Mesh"][0]);
            gH_Layout.AddTab(categoryDict["Intersect"][0]);
            gH_Layout.AddTab(categoryDict["Transform"][0]);
            gH_Layout.AddTab(categoryDict["Display"][0]);
            foreach (IGH_ObjectProxy objectProxy in Instances.ComponentServer.ObjectProxies)
            {
                GH_Exposure gH_Exposure = objectProxy.Exposure;
                if (gH_Exposure != GH_Exposure.hidden)
                {
                    if (gH_Exposure == GH_Exposure.obscure)
                    {
                        gH_Exposure = GH_Exposure.septenary | GH_Exposure.obscure;
                    }
                    gH_Layout.AddItem(objectProxy.Desc.Category, objectProxy.Desc.SubCategory, objectProxy.Guid, gH_Exposure);
                }
            }
            foreach (GH_LayoutTab tab in gH_Layout.Tabs)
            {
                tab.Panels.Sort();
                foreach (GH_LayoutPanel panel in tab.Panels)
                {
                    panel.Sort();
                }
            }
            //gH_Layout.FindTab("Params")?.SortPanels("Geometry", "Primitive", "Input", "Util");
            return gH_Layout;
        }
        #endregion


        #endregion


        #region Write XML

        /// <summary>
        /// Write to the right languages to the files.
        /// </summary>
        /// <param name="info"></param>
        public static void WriteXml(CultureInfo info)
        {
            string directory = $"{Path}\\{info.Name}";
            Directory.CreateDirectory(directory);

            Dictionary<string, SortedSet<string>> cateSubSets = new Dictionary<string, SortedSet<string>>();

            //Save Every Proxy, but maybe not so efficient.
            Instances.ComponentServer.ObjectProxies.Cast<GH_LanguageObjectProxy>().ToList().ForEach((proxy) => proxy.Save());

            //Save Cateogry Languages.
            string fileName = $"_{info.Name}_Category.xml";
            CreateXmlDocumentAndSave(directory, fileName, (doc) => CategoriesToXml(doc));

        }

        /// <summary>
        /// Create a XmlDocument, Please Create the root XmlElement base on the XmlDocument. And then, it will save the whole Document.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <param name="getElement"></param>
        internal static void CreateXmlDocumentAndSave(string directory, string fileName, Func<XmlDocument, XmlElement> getElement)
        {
            string location = $"{directory}\\{fileName}";
            string sha = UnsafeHelper.HashString(fileName);

            XmlDocument doc = new XmlDocument();
            XmlElement element = getElement(doc);
            element.SetAttribute("HASH", sha);

            doc.AppendChild(element);
            doc.Save(location);
        }

        #region CategoryToXml
        private static XmlElement CategoriesToXml(XmlDocument doc)
        {
            
            SortedList<string, string> shortCategories = (SortedList<string, string>)typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categoryShort")).First().GetValue(Grasshopper.Instances.ComponentServer);
            SortedList<string, string> symbolCategories = (SortedList<string, string>)typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categorySymbol")).First().GetValue(Grasshopper.Instances.ComponentServer);

            XmlElement xmlElement = doc.CreateElement("Categories");
            xmlElement.SetAttribute("Translator", string.Empty);
            foreach (var pair in _oldCateSubSets)
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

        #endregion
    }
}
