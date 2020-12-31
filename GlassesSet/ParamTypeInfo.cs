/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace InfoGlasses
{
    //public class ParamTypeInfo
    //{
    //    public Bitmap Icon { get; set; }
    //    public BitmapSource IconSource { get; set; }

    //    public string TypeFullName { get; set; }
    //    public String Name { get; set; }

    //    public string AssemblyName { get; set; }

    //    public string AssemblyDesc { get; set; }
    //    public string ParamFullName { get; set; }
    //    public string ProxyDesc { get; set; }
    //    public BitmapSource AssemblyIconSource { get; set; }

    //    public bool IsPlugin { get; set; }

    //    public bool HasInput { get; set; }

    //    public Color ShowColor { get; set; }

    //    private void CreateAttribute(IGH_Param param)
    //    {
    //        this.Icon = param.Icon_24x24;
    //        this.IconSource = CanvasRenderEngine.BitmapToBitmapImage(new Bitmap(this.Icon, 20, 20));
    //        this.ProxyDesc = param.Description;


    //        this.HasInput = param.IsDataProvider;
    //        Type type = param.Type;

    //        this.TypeFullName = type.FullName;

    //        //void ScheduleCallback(GH_Document doc)
    //        //{
    //        this.Name = param.Type.Name;
    //        //}
    //        //GH_Document.GH_ScheduleDelegate callback = new GH_Document.GH_ScheduleDelegate(ScheduleCallback);
    //        //document.ScheduleSolution(10, callback);


    //        this.ParamFullName = param.GetType().FullName;

    //        //this.AllInstances = new List<IGH_Param>();
    //        GH_AssemblyInfo assem = ComTypeInfo.GetGHLibrary(param.GetType());
    //        this.AssemblyName = assem.Assembly.GetName().Name;
    //        this.AssemblyDesc = assem.Description;

    //        this.IsPlugin = !assem.IsCoreLibrary;
    //        if (assem.Icon != null)
    //            this.AssemblyIconSource = CanvasRenderEngine.BitmapToBitmapImage(new Bitmap(assem.Icon, 16, 16));
    //        else
    //            this.AssemblyIconSource = CanvasRenderEngine.BitmapToBitmapImage(new Bitmap(16, 16));
    //    }

    //    public ParamTypeInfo(IGH_ObjectProxy proxy)
    //    {

    //        IGH_Param param = ((IGH_Param)proxy.CreateInstance());
    //        CreateAttribute(param);
    //    }

    //    public ParamTypeInfo(IGH_Param param)
    //    {
    //        CreateAttribute(param);
    //    }




    //}
}
