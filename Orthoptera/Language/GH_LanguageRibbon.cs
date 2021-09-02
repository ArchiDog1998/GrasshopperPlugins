/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Grasshopper;
using Grasshopper.Kernel;
using Microsoft.VisualBasic.CompilerServices;
using Rhino;

namespace Orthoptera.Language
{
    public class GH_LanguageRibbon: GH_Ribbon
    {
        #region Change Category

        public void NewPopulateRibbon()
		{
			FieldInfo _populateCounterInfo = typeof(GH_Ribbon).GetRuntimeFields().Where((field) => field.Name.Contains("_populateCounter")).First();
			FieldInfo m_tabsInfo = typeof(GH_Ribbon).GetRuntimeFields().Where((field) => field.Name.Contains("m_tabs")).First();
			MethodInfo EnsureTabInfo = typeof(GH_Ribbon).GetRuntimeMethods().Where((method) => method.Name.Contains("EnsureTab")).First();

			int _populateCounter = (int)_populateCounterInfo.GetValue(this);
			List<GH_RibbonTab> m_tabs = (List<GH_RibbonTab>)m_tabsInfo.GetValue(this);

			if (base.DesignMode)
			{
				_populateCounter--;
				return;
			}
			if (_populateCounter > 32)
			{
				throw new InvalidProgramException("Ribbon could not be populated 32 times in a row.");
			}
			try
			{
				string activeTabName = ActiveTabName;
				Tabs.Clear();
				List<GH_Layout> list = new List<GH_Layout>();
				list.Add(GetLayout());
				foreach (GH_Layout item in list)
				{
					foreach (GH_LayoutTab tab in item.Tabs)
					{
						GH_RibbonTab gH_RibbonTab = (GH_RibbonTab)EnsureTabInfo.Invoke(this, new object[] { tab.Name });
						foreach (GH_LayoutPanel panel in tab.Panels)
						{
							GH_RibbonPanel gH_RibbonPanel = gH_RibbonTab.EnsurePanel(panel.Name);
							foreach (GH_LayoutItem item2 in panel.Items)
							{
								if (!gH_RibbonPanel.Contains(item2.Id, item2.Exposure))
								{
									IGH_ObjectProxy iGH_ObjectProxy = Instances.ComponentServer.EmitObjectProxy(item2.Id);
									if (iGH_ObjectProxy != null)
									{
										iGH_ObjectProxy = iGH_ObjectProxy.DuplicateProxy();
										iGH_ObjectProxy.Exposure = item2.Exposure;
										iGH_ObjectProxy.Desc.Category = tab.Name;
										iGH_ObjectProxy.Desc.SubCategory = panel.Name;
										gH_RibbonPanel.AddItem(new GH_RibbonItem(iGH_ObjectProxy));
									}
								}
							}
						}
					}
				}
				if (m_tabs.Count == 0)
				{
					_populateCounter--;
					return;
				}
				m_tabs[0].Visible = true;
				foreach (GH_RibbonTab tab2 in m_tabs)
				{
					foreach (GH_RibbonPanel panel2 in tab2.Panels)
					{
						panel2.Sort();
					}
				}
				ActiveTabName = activeTabName;
				LayoutRibbon();
				Refresh();
				_populateCounter--;
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				_populateCounter++;
				RhinoApp.InvokeOnUiThread(new Action(PopulateRibbon), null);
				ProjectData.ClearProjectError();
			}
		}

		private static GH_Layout GetLayout()
        {
			GH_Layout gH_Layout = new GH_Layout();
			gH_Layout.FilePath = "Default";
			foreach (IGH_ObjectProxy objectProxy in Instances.ComponentServer.ObjectProxies)
			{
				GH_Exposure gH_Exposure = objectProxy.Exposure;
				if (gH_Exposure != GH_Exposure.hidden)
				{
					if (gH_Exposure == GH_Exposure.obscure)
					{
						gH_Exposure = GH_Exposure.septenary | GH_Exposure.obscure;
					}
					gH_Layout.AddItem(objectProxy.Desc.Category, objectProxy.Desc.SubCategory, objectProxy.Guid, gH_Exposure);
				}
			}
			foreach (GH_LayoutTab tab in gH_Layout.Tabs)
			{
				tab.Panels.Sort();
				foreach (GH_LayoutPanel panel in tab.Panels)
				{
					panel.Sort();
				}
			}
			//gH_Layout.FindTab("Params")?.SortPanels("Geometry", "Primitive", "Input", "Util");
			return gH_Layout;
		}
        #endregion
    }
}
