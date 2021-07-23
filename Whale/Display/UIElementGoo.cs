using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Whale.Display
{
    public class UIElementGoo : GH_Goo<UIElement>
    {
        public override bool IsValid => Value != null;

        public override string TypeName => "UIElement";

        public override string TypeDescription => "UIElement for WPF.";

        public UIElementGoo()
        {
        }

        public UIElementGoo(UIElement element)
            : base(element)
        {
        }

        public override IGH_Goo Duplicate()
        {
            if (Value == null)
            {
                return new UIElementGoo(null);
            }
            return new UIElementGoo(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool CastFrom(object source)
        {
            UIElement element = source as UIElement;
            if (element != null)
            {
                Value = element;
                return true;
            }
            return false;
        }

        public override bool CastTo<TQ>(ref TQ target)
        {
            if (typeof(UIElement).IsAssignableFrom(typeof(TQ)))
            {
                target = (TQ)(object)Value;
                return true;
            }
            return false;
        }
    }
}
