/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper
{
    public static class ColorExtension
    {
        public static Color LightenColor(this Color color, int change)
        {
            int R = color.R + change;
            int G = color.G + change;
            int B = color.B + change;

            R = R > 255 ? 255 : R;
            G = G > 255 ? 255 : G;
            B = B > 255 ? 255 : B;

            R = R < 0 ? 0 : R;
            G = G < 0 ? 0 : G;
            B = B < 0 ? 0 : B;

            return Color.FromArgb(color.A, R, G, B);
        }

        public static Color SolidenColor(this Color color, int change)
        {
            int A = color.A + change;

            A = A > 255 ? 255 : A;

            A = A < 0 ? 0 : A;

            return Color.FromArgb(A, color);
        }

        public static Color OnColor = Color.FromArgb(255, 19, 34, 122);
        public static System.Windows.Media.Color MediaOnColor = ConvertToMediaColor(OnColor);
        public static Color OffColor = Color.FromArgb(255, 60, 60, 60);
        public static System.Windows.Media.Color MediaOffColor = ConvertToMediaColor(OffColor);

        public static Color UnableColor = Color.FromArgb(255, 100, 100, 100);

        public static System.Windows.Media.Color ConvertToMediaColor(Color color)
        {
            System.Windows.Media.Color outColor = new System.Windows.Media.Color();
            outColor.A = color.A;
            outColor.R = color.R;
            outColor.G = color.G;
            outColor.B = color.B;

            return outColor;
        }
    }
}
