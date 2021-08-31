/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Microsoft.VisualBasic.CompilerServices;
using Rhino;

namespace Orthoptera.Language
{
    internal class GH_LanguageCompiledProxy : IGH_ObjectProxy
    {

        public string Location { get; }

        public Guid LibraryGuid { get; }

        public bool SDKCompliant { get; }

        public bool Obsolete { get; }

        public Type Type { get; }

        public GH_ObjectType Kind { get; }

        public Guid Guid { get; }

        public Bitmap Icon { get; }

        public IGH_InstanceDescription Desc { get; }

        public GH_Exposure Exposure { get; set; }

        public List<string[]> InputParams { get; set; }
        public List<string[]> OutputParams { get; set; }


        internal GH_LanguageCompiledProxy(IGH_ObjectProxy proxy, IGH_InstanceDescription desc)
        {
            this.Location = proxy.Location;
            this.LibraryGuid = proxy.Guid;
            this.SDKCompliant = proxy.SDKCompliant;
            this.Obsolete = proxy.Obsolete;
            this.Type = proxy.Type;
            this.Kind = proxy.Kind;
            this.Guid = proxy.Guid;
            this.Icon = proxy.Icon;
            this.Desc = desc;
            this.Exposure = proxy.Exposure;
        }

        private void Translate(ref IGH_DocumentObject obj, bool isTranslateParamNameAndNick)
        {
            obj.Name = Desc.Name;
            obj.NickName = Desc.NickName;
            obj.Description = Desc.Description;
            if (Desc.HasCategory)
                obj.Category = Desc.Category;
            if (Desc.HasSubCategory)
                obj.SubCategory = Desc.SubCategory;

            if(obj is GH_Component)
            {
                GH_Component com = obj as GH_Component;
                for (int i = 0; i < com.Params.Input.Count; i++)
                {
                    com.Params.Input[i].Description = InputParams[i][2];
                    if (isTranslateParamNameAndNick)
                    {
                        com.Params.Input[i].Name = InputParams[i][0];
                        com.Params.Input[i].NickName = InputParams[i][1];
                    }
                }
                for (int i = 0; i < com.Params.Output.Count; i++)
                {
                    com.Params.Output[i].Description = OutputParams[i][2];
                    if (isTranslateParamNameAndNick)
                    {
                        com.Params.Output[i].Name = OutputParams[i][0];
                        com.Params.Output[i].NickName = OutputParams[i][1];
                    }
                }
            }
        }

        public IGH_DocumentObject CreateInstance()
        {
            try
            {
                switch (this.Kind)
                {
                    case GH_ObjectType.CompiledObject:
                        object objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(Type));
                        if (objectValue == null)
                        {
                            return null;
                        }
                        IGH_DocumentObject obj = objectValue as IGH_DocumentObject;
                        Translate(ref obj, true);
                        return obj;

                    case GH_ObjectType.UserObject:
                        GH_UserObject gH_UserObject = new GH_UserObject();
                        gH_UserObject.Path = Location;
                        gH_UserObject.ReadFromFile();

                        IGH_DocumentObject user = gH_UserObject.InstantiateObject();
                        Translate(ref user, false);
                        return user;
                    default:
                        return null;

                }

            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                IGH_DocumentObject result = null;
                ProjectData.ClearProjectError();
                return result;
            }
        }

        public IGH_ObjectProxy DuplicateProxy() => new GH_LanguageCompiledProxy(this, this.Desc);
    }
}
