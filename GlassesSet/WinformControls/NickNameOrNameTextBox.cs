using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiTed_Grasshopper.WinformControls;
using static InfoGlasses.WinformMenu.InfoGlassesMenuItem;

namespace InfoGlasses.WinformControls
{
    /// <summary>
    /// Show the component's nickname or fullname.
    /// </summary>
/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

    public sealed class NickNameOrNameTextBox : TedTextBox
    {
        private bool IsShowNickName => (bool)Settings.GetProperty(InfoGlassesProps.IsShowNickName);

        /// <summary>
        /// Show the component's nickname or fullname.
        /// </summary>
        /// <param name="target"> the GH_DocumentObject that relay on.  </param>
        /// <param name="layout"> How to define the bounds. </param>
        /// <param name="meansureString"> get the string's bounds. </param>
        /// <param name="showFunc"> whether to show. </param>
        /// <param name="renderLittleZoom"> whether render when zoom is less than 0.5. </param>
        public NickNameOrNameTextBox(IGH_DocumentObject target, Func<SizeF, RectangleF, RectangleF> layout,
            Func<Graphics, string, Font, SizeF> meansureString = null, Func<bool> showFunc = null, bool renderLittleZoom = false)
            : base(null, target, layout, new NameBoxRenderSet(), meansureString, showFunc, renderLittleZoom)
        {
        }

        public override string ShowName
        {
            get
            {
                string name = this.IsShowNickName ? Target.NickName : Target.Name;
                if (string.IsNullOrEmpty(name))
                    return " ";
                else
                    return name;
            }
        }
    }
}
