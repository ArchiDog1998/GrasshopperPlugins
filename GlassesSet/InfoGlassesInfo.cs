/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using InfoGlasses.WinformMenu;

namespace InfoGlasses
{
    public class InfoGlassesInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "ShowcaseTools";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Easy to showcase.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("469ffc23-67f8-42ae-85c1-016205de9cbe");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "秋水";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "1123993881@qq.com";
            }
        }

        public override string Version
        {
            get
            {
                return "1.2.8.v" + DateTime.Now.ToString("yyyyMMdd");
            }
        }
    }

    public class ShowcaseToolsPrior : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;
            return GH_LoadingInstruction.Proceed;
        }

        private void Instances_CanvasCreated(GH_Canvas canvas)
        {
            Grasshopper.Instances.CanvasCreated -= Instances_CanvasCreated;

            GH_DocumentEditor editor = Grasshopper.Instances.DocumentEditor;
            if (editor == null) return;

            editor.MainMenuStrip.Items.Add(new ShowcaseToolsMenu().CreateMajor());
        }
    }
}
