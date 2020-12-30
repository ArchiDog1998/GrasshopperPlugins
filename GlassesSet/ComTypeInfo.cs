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
using System.Windows.Media;

namespace InfoGlasses
{
    public class ComTypeInfo
    {
        public string Category { get; set; }
        public string Subcategory { get; set; }

        public GH_Exposure Exposure { get; set; }
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public object Tip { get; set; }
        public Guid Guid { get; set; }

        public bool Isvalid { get; set; }
        public string FullName { get; set; }
        public string ShowLocation { get; set; }
        public bool IsPlugin { get; set; }
        public string PluginLocation { get; set; }
        public string Description { get; set; }

        public string AssemblyName { get; set; }
        public string AssemblyAuthor { get; set; }

        public ComTypeInfo(IGH_ObjectProxy proxy)
        {
            this.Category = proxy.Desc.HasCategory ? proxy.Desc.Category : LanguagableComponent.GetTransLation(new string[] { "<Unnamed Category>", "<未命名类别>" });
            this.Subcategory = proxy.Desc.HasSubCategory ? proxy.Desc.SubCategory : LanguagableComponent.GetTransLation(new string[] { "<Unnamed Subcategory>", "<未命名子类>" });
            this.Name = proxy.Desc.Name;
            this.Description = proxy.Desc.Description;
            #region
            Bitmap bitmap = new Bitmap(proxy.Icon, 20, 20);
            this.Icon = CanvasRenderEngine.BitmapToBitmapImage(bitmap);
            #endregion

            this.Exposure = proxy.Exposure;
            this.Guid = proxy.Guid;

            Isvalid = true;

            Type type = proxy.Type;
            GH_AssemblyInfo lib = null;

            if (type == null)
                this.FullName = "";
            else
            {
                this.FullName = type.FullName;
                lib = GetGHLibrary(type);
            }

            this.IsPlugin = checkisPlugin(proxy, lib);

            if (lib != null)
            {
                this.AssemblyName = lib.Name;
                this.AssemblyAuthor = lib.AuthorName;
            }

            this.Tip = CreateTip();
        }
        public ComTypeInfo(IGH_DocumentObject obj)
        {
            this.Category = obj.HasCategory ? obj.Category : LanguagableComponent.GetTransLation(new string[] { "<Unnamed Category>", "<未命名类别>" });
            this.Subcategory = obj.HasSubCategory ? obj.SubCategory : LanguagableComponent.GetTransLation(new string[] { "<Unnamed Subcategory>", "<未命名子类>" });
            this.Name = obj.Name;
            this.Description = obj.Description;
            #region
            Bitmap bitmap = new Bitmap(obj.Icon_24x24, 20, 20);
            this.Icon = CanvasRenderEngine.BitmapToBitmapImage(bitmap);
            #endregion

            this.Exposure = obj.Exposure;
            this.Guid = obj.ComponentGuid;


            Type type = obj.GetType();
            GH_AssemblyInfo lib = null;

            Isvalid = true;

            if (type == null)
                this.FullName = "";
            else
            {
                this.FullName = type.FullName;
                lib = GetGHLibrary(type);
            }

            this.PluginLocation = lib.Location;
            this.ShowLocation = this.PluginLocation;
            this.IsPlugin = !lib.IsCoreLibrary;

            if (lib != null)
            {
                this.AssemblyName = lib.Name;
                this.AssemblyAuthor = lib.AuthorName;
            }

            this.Tip = CreateTip();
        }

        private object CreateTip()
        {

            string result = "";
            result = this.Name;
            result += "\n" + this.Description;
            if (this.AssemblyName != null)
                result += ("\n \n" + this.AssemblyName + "        " + this.AssemblyAuthor);


            return result;
        }

        private bool checkisPlugin(IGH_ObjectProxy proxy, GH_AssemblyInfo lib)
        {
            PluginLocation = proxy.Location;
            if (lib == null)
            {
                ShowLocation = PluginLocation;
                return true;
            }

            string loc2 = lib.Location;


            if (PluginLocation != loc2)
            {
                ShowLocation = loc2 + "\n \n" + PluginLocation;
                return true;
            }
            else
            {
                ShowLocation = PluginLocation;
                return !lib.IsCoreLibrary;
            }
        }

        public static GH_AssemblyInfo GetGHLibrary(Type type)
        {
            foreach (GH_AssemblyInfo lib in Grasshopper.Instances.ComponentServer.Libraries)
            {
                if (lib.Assembly == type.Assembly)
                {
                    return lib;
                }
            }
            return null;
        }

        protected void GetObjectAssemblyLoc(IGH_DocumentObject docObject, ref string loc, ref string info)
        {
            Type type = docObject.GetType();
            info = type.FullName;

        }
    }

    public class ComInfo
    {
        public IGH_DocumentObject Self { get; set; }

        public bool IsShowNormal { get; set; }
        public bool IsShowPlugin { get; set; }
        public ComTypeInfo Typeinfo { get; set; }

        public string FullName { get; set; }
        public string ShowLocation { get; set; }

        public ComInfo(IGH_DocumentObject obj, List<ComTypeInfo> infoList, List<ComTypeInfo> exceptNormal, List<ComTypeInfo> exceptPlugin)
        {
            this.Self = obj;
            this.Typeinfo = FindTypeInfo(obj, infoList);

            if (this.Typeinfo.FullName == "")
            {
                this.FullName = obj.GetType().FullName;


                this.ShowLocation = this.Typeinfo.ShowLocation + "\n \n" + ComTypeInfo.GetGHLibrary(obj.GetType()).Location;
            }
            else
            {
                this.FullName = this.Typeinfo.FullName;
                this.ShowLocation = this.Typeinfo.ShowLocation;
            }
            UpdateIs(exceptNormal, exceptPlugin);

        }

        private ComTypeInfo FindTypeInfo(IGH_DocumentObject obj, List<ComTypeInfo> infoList)
        {
            foreach (ComTypeInfo info in infoList)
            {
                if (obj.Name == info.Name && obj.Description == info.Description)
                    return info;
            }

            foreach (ComTypeInfo info in infoList)
            {
                if (obj.ComponentGuid == info.Guid)
                    return info;
            }

            //System.Windows.Forms.MessageBox.Show(obj.Name + "|" + obj.Description);
            ComTypeInfo typeinfo = new ComTypeInfo(obj);
            infoList.Add(typeinfo);
            return typeinfo;
        }

        public void UpdateIs(List<ComTypeInfo> exceptNormal, List<ComTypeInfo> exceptPlugin)
        {
            this.IsShowNormal = !exceptNormal.Contains(Typeinfo);
            this.IsShowPlugin = !exceptPlugin.Contains(Typeinfo);
        }

    }
}
