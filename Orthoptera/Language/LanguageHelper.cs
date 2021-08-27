/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;

namespace Orthoptera.Language
{
    public static class LanguageHelper
    {
        public static void CreateLanguageXml(IList<IGH_ObjectProxy> proxies, CultureInfo info)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("LanguageDetails");

            #region Save Object Descriptions
            XmlElement objectElement = doc.CreateElement("DocumentObjects");
            foreach (var proxy in proxies)
            {
                objectElement.AppendChild(GH_ObjectDescription.CreateFromProxy(proxy).ToXml(doc));
            }

            element.AppendChild(objectElement);
            #endregion

            #region Save Category Descriptions
            #endregion

            #region Save Menu Description
            #endregion

            doc.AppendChild(element);
            doc.Save(Grasshopper.Folders.AppDataFolder + info.Name + ".xml");
        }
    }
}
