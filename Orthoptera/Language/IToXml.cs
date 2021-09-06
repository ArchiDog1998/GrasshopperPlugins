/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Orthoptera.Language
{
    public interface IToXml
    {
        XmlElement ToXml(XmlDocument doc, CultureInfo culture);

        void Refresh(XmlElement element);
    }
}
