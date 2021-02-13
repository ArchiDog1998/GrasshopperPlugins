/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace TestProject
{
    public class TestProjectInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "TestProject";
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
                return "A project for test.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("a9dcd59e-69e2-48ba-9b41-dd881d5c04ee");
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
}
