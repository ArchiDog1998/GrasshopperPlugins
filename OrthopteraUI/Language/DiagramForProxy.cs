/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Globalization;
using System.Xml;
using Orthoptera.Language;
using System.IO;

namespace OrthopteraUI.Language
{
    public struct DiagramForProxy: IToXml
    {
        public Image Picture { get; set; }
        public string Description { get; set; }

        public DiagramForProxy(Bitmap map, string desc)
        {
            this.Picture = map;
            this.Description = desc;
        }

        public XmlElement ToXml(XmlDocument doc, CultureInfo culture)
        {
            XmlElement xmlElement = doc.CreateElement(nameof(DiagramForProxy));
            xmlElement.InnerText = BitmapBase64ToString(Picture);
            xmlElement.SetAttribute(nameof(Description), Description);
            return xmlElement;
        }



        public void Refresh(XmlElement element)
        {
            if (element.Name != nameof(DiagramForProxy))
                throw new ArgumentException("Element's Name must be " + nameof(DiagramForProxy));
            this.Picture = BitmapBase64FromString(element.InnerText);
            this.Description = element.GetAttribute(nameof(Description));
        }

        public static string BitmapBase64ToString(Image map)
        {
            MemoryStream stream = new MemoryStream();
            map.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imageBytes = stream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        public static Image BitmapBase64FromString(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            MemoryStream stream = new MemoryStream(bytes);
            return Image.FromStream(stream);
        }
    }
}
