/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Grasshopper.Kernel;
using System.Xml;

namespace Orthoptera.Language
{
    internal class GH_CategoryDescription
    {
        private static string XmlName => "Category";
        private static string xmlName => "SubCategory";

        internal readonly static FieldInfo IconsField = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categoryIcons")).First();
        internal readonly static FieldInfo ShortField = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categoryShort")).First();
        internal readonly static FieldInfo SymbolField = typeof(GH_ComponentServer).GetRuntimeFields().Where((field) => field.Name.Contains("_categorySymbol")).First();


        internal string Category { get; set; }
        internal string Short { get; set; }
        internal string Symbol { get; set; }
        internal string Translator { get; set; } = "";

        internal string CategoryFullName { get; set; }
        internal List<string> Subcategories { get; private set; } = new List<string>();

        internal GH_CategoryDescription(string cate, string shortCate, string symbol, List<string> subCates)
        {
            this.Category = cate;
            this.Short = shortCate;
            this.Symbol = symbol;
            this.Subcategories = subCates;
            this.CategoryFullName = cate;
        }

        #region XML
        internal GH_CategoryDescription(XmlElement element)
        {
            this.Translator = element.GetAttribute(nameof(this.Translator));
            this.Category = element.GetAttribute(nameof(this.Category));
            this.Short = element.GetAttribute(nameof(this.Short));
            this.Symbol = element.GetAttribute(nameof(this.Symbol));
            this.CategoryFullName = element.GetAttribute(nameof(this.CategoryFullName));

            foreach (var obj in element.ChildNodes)
            {
                XmlNode node = (XmlNode)obj;
                if (node.Name == nameof(xmlName))
                {
                    this.Subcategories.Add(((XmlElement)node).GetAttribute(xmlName));
                }
            }
        }

        internal XmlElement ToXml(XmlDocument doc)
        {
            XmlElement xmlElement = doc.CreateElement(XmlName);
            xmlElement.SetAttribute(nameof(this.Translator), this.Translator);
            xmlElement.SetAttribute(nameof(this.Category), this.Category);
            xmlElement.SetAttribute(nameof(this.Short), this.Short);
            xmlElement.SetAttribute(nameof(this.Symbol), this.Symbol);
            xmlElement.SetAttribute(nameof(this.CategoryFullName), this.CategoryFullName);

            foreach (var sub in Subcategories)
            {
                XmlElement subXmlElement = doc.CreateElement(xmlName);
                subXmlElement.SetAttribute(xmlName, sub);
                xmlElement.AppendChild(subXmlElement);
            }

            return xmlElement;
        }
        #endregion

    }
}
