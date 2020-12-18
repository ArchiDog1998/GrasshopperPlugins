using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Whale
{
    public class WhalesInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Whale";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Properties.Resources.Whales;
            }
        }

        public override string Version
        {
            get
            {
                return "0.9.9.20200912";
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Whale for Animation and Visualization.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("c99762ab-f99c-49e6-b549-d4f93e8e1514");
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
    }

    public class WhalesIcon : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("Whale", Properties.Resources.Whales_16);
            Grasshopper.Instances.ComponentServer.AddCategoryShortName("Whale", "Wh");
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Whale", 'W');
            return GH_LoadingInstruction.Proceed;
        }
    }

}
