using ArchiTed_Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoGlasses.WinformMenu
{
    public class ParamGlassesMenuItem : ToolStripMenuItem
    {
        public enum ParamGlassesProps
        {
            IsUseParamGlasses,

            IsUseWireColor,
            WireColorsDict,
            WireCheckStrongth,
            WireDefaultColor,
            WireSelectedColor,
            WireEmptyColor,
            WireType,
        }
        public static SaveableSettings<ParamGlassesProps> Settings { get; } = new SaveableSettings<ParamGlassesProps>(new SettingsPreset<ParamGlassesProps>[]
        {
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.IsUseParamGlasses, true),

            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.IsUseWireColor, true, (value)=>
            {
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireColorsDict, new Dictionary<string, Color>()),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireCheckStrongth, 0.0, (value)=> Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireDefaultColor, Color.FromArgb(150, 0, 0, 0), (value)=>
            {
                GH_Skin.wire_default = (Color)value;
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireSelectedColor, Color.FromArgb(125, 210, 40), (value)=>                 
            {
                GH_Skin.wire_selected_a = GH_Skin.wire_selected_b = (Color)value;
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireEmptyColor, Color.FromArgb(180, 255, 60, 0), (value) =>
            {
                GH_Skin.wire_empty = (Color)value;
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireType, 0, (value) => Grasshopper.Instances.ActiveCanvas.Refresh()),


        }, Grasshopper.Instances.Settings);

        public ParamGlassesMenuItem() : base(Properties.Resources.WireGlasses)
        {
            this.SetItemLangChange(new string[] { "ParamGlasses", "参数眼镜" }, new string[]{"You can use it to see the extra infomation about parameters.",
                "你可以用它查看参数的更多信息。"});
            this.BindingAndCheckProperty(Settings, ParamGlassesProps.IsUseParamGlasses, (value) =>
            {
                //RemovePaintActions();
                //if (value)
                //    AddPaintActions();
                Grasshopper.Instances.ActiveCanvas.Refresh();
            });

            WinFormPlus.ItemSet<ParamGlassesProps>[] itemsets = new WinFormPlus.ItemSet<ParamGlassesProps>[]
            {
                new WinFormPlus.ItemSet<ParamGlassesProps>(new string[]{ "Default Color", "默认颜色"}, null, null ,true ,ParamGlassesProps.WireDefaultColor),
                new WinFormPlus.ItemSet<ParamGlassesProps>(new string[]{ "Selected Color", "选中颜色"}, null, null ,true ,ParamGlassesProps.WireSelectedColor),
                new WinFormPlus.ItemSet<ParamGlassesProps>(new string[]{ "Empty Color", "空值颜色"}, null, null ,true ,ParamGlassesProps.WireEmptyColor),

            };
            this.DropDown.Items.Add(WinFormPlus.CreateColorBoxItems(Settings,
                new string[] { "Default Wire Color", "默认连线颜色" }, null, null, true, itemsets));

            this.DropDown.Items.Add(WinFormPlus.CreateNumberBox(new string[] { "Wire Strongth", "选中加重" }, null, null,
                Settings, ParamGlassesProps.WireCheckStrongth, 20, 0));

            this.DropDown.Items.Add(WinFormPlus.CreateComboBoxItemSingle(new string[] { "Wire Type", "连线类型" }, null, null, 
                Settings, ParamGlassesProps.WireType, new string[][]
            {
                new string[]{ "Bezier Curve", "贝塞尔曲线" },
                new string[] { "PolyLine", "多段线" },
                new string[] { "Line", "直线" },
            }));

            GH_DocumentObject.Menu_AppendSeparator(this.DropDown);

            this.DropDown.Items.Add(GetShowWireColorItem());

        }

        #region Department
        private ToolStripMenuItem GetShowWireColorItem()
        {
            ToolStripMenuItem item = WinFormPlus.CreateCheckItem(new string[] { "Use Advanced Wire Color", "使用强化连线颜色" },
                new string[] { "Click to show use advanced wire color.", "单击以显示强化连线颜色。" },
                Properties.Resources.WireIcon, Settings, ParamGlassesProps.IsUseWireColor);

            item.DropDown.Items.Add(WinFormPlus.CreateClickItem(new string[] { "WireColor", "连线颜色" },
                new string[] { "Click to open the wirecolor window.", "单击以打开连线颜色窗口。" }, null, (x, y) =>
                {
                    //new ExceptionWindowPlus(this).Show();
                }));

            return item;
        }
        #endregion
    }
}
