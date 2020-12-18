using Grasshopper.GUI.Base;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
	internal class InputBoxBalloon : GH_TextBoxInputBase
	{

		public Action<string> SetValue { get; }


		public InputBoxBalloon(RectangleF bounds, Action<string> setValue)
		{
			this.SetValue = setValue;

			Bounds = GH_Convert.ToRectangle(bounds);
			Font = GH_FontServer.ConsoleAdjusted;
		}

		protected override void HandleTextInputAccepted(string text)
		{
			SetValue(text);
		}
	}
}
