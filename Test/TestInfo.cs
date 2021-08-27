using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Test
{
    public class TestInfo : GH_AssemblyInfo
    {
        public override string Name => "Test";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("6B69FC59-9E52-4E23-879C-2ACFA80F2FFA");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}