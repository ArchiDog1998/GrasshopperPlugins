using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Whale.Rect
{
    class RectSolution
    {
        public static List<Rectangle3d> VerticalTiles(Rectangle3d B, List<double> R, double W, double WP, double HP, out Rectangle3d rest)
        {

            var m_tileSet = new List<Rectangle3d>();
            Plane m_basePlane = new Plane(B.Corner(0), B.Plane.XAxis, B.Plane.YAxis);

            var m_available = B.Height - (HP * B.Height);

            double m_total = R.Sum();
            double m_hPadding = (HP * B.Height) / (R.Count + 1);
            double m_wPadding = (WP * B.Width) / 2; ;
            double m_width = B.Width * W - (WP * B.Width) / 2;

            double m_yValue = 0;

            for (int i = 0; i < R.Count; i++)
            {
                m_yValue += m_hPadding;
                double m_nextY = m_yValue + (R[i] / m_total) * m_available;

                Rectangle3d relayRect = new Rectangle3d(m_basePlane,
                    m_basePlane.PointAt(m_wPadding, m_yValue, 0),
                    m_basePlane.PointAt(m_width, m_nextY, 0));
                m_tileSet.Add(new Rectangle3d(new Plane(relayRect.Corner(0), relayRect.Plane.XAxis, relayRect.Plane.YAxis), relayRect.Width, relayRect.Height));
                m_yValue = m_nextY;
            }

            rest = new Rectangle3d(m_basePlane, new Interval(B.Width * W, B.Width), new Interval(0, B.Height));

            return m_tileSet;
        }

        public static List<Rectangle3d> HorizontalTiles(Rectangle3d B, List<double> R, double H, double WP, double HP, out Rectangle3d rest)
        {
            var m_tileSet = new List<Rectangle3d>();
            Plane m_basePlane = new Plane(B.Corner(0), B.Plane.XAxis, B.Plane.YAxis);

            var m_available = B.Width - (WP * B.Width);

            double m_total = R.Sum();
            double m_hPadding = (HP * B.Height) / 2;
            double m_wPadding = (WP * B.Width) / (R.Count + 1);
            double m_height = B.Height * H - (HP * B.Height) / 2;

            double m_xValue = 0;

            for (int i = 0; i < R.Count; i++)
            {
                m_xValue += m_wPadding;
                double m_nextX = m_xValue + (R[i] / m_total) * m_available;
                Rectangle3d relayRect = new Rectangle3d(m_basePlane,
                    m_basePlane.PointAt(m_xValue, m_hPadding, 0),
                    m_basePlane.PointAt(m_nextX, m_height, 0));
                m_tileSet.Add(new Rectangle3d(new Plane(relayRect.Corner(0), relayRect.Plane.XAxis, relayRect.Plane.YAxis), relayRect.Width, relayRect.Height));
                m_xValue = m_nextX;
            }

            rest = new Rectangle3d(m_basePlane, new Interval(0, B.Width), new Interval(B.Height * H, B.Height));
            return m_tileSet;
        }

        public static List<Rectangle3d> GridTiles(Rectangle3d B, double IP, double EP, int C, int R, int PA)
        {


            var m_tileSet = new List<Rectangle3d>();
            Plane m_basePlane = new Plane(B.Corner(0), B.Plane.XAxis, B.Plane.YAxis);

            double m_padSource = (PA == 0 ? B.Width : B.Height);
            var m_iPadding = IP * m_padSource;
            var m_ePadding = EP * m_padSource;

            var m_cellWidth = (B.Width - (m_iPadding * (C - 1)) - (m_ePadding * 2)) / C;
            var m_cellHeight = (B.Height - (m_iPadding * (R - 1)) - (m_ePadding * 2)) / R;

            for (int i = 0; i < R; i++)
            {
                double m_llY = m_ePadding + m_cellHeight * i + m_iPadding * i;
                double m_urY = m_ePadding + (m_cellHeight * (i + 1)) + m_iPadding * i;
                for (int j = 0; j < C; j++)
                {
                    double m_llX = m_ePadding + m_cellWidth * j + m_iPadding * j;
                    double m_urX = m_ePadding + (m_cellWidth * (j + 1)) + m_iPadding * j;
                    Rectangle3d relayRect = new Rectangle3d(m_basePlane, m_basePlane.PointAt(m_llX, m_llY, 0), m_basePlane.PointAt(m_urX, m_urY, 0));
                    m_tileSet.Add(new Rectangle3d(new Plane(relayRect.Corner(0), relayRect.Plane.XAxis, relayRect.Plane.YAxis), relayRect.Width, relayRect.Height));
                }

            }
            return m_tileSet;
        }

        public static List<Rectangle3d> IrregularTiles(Rectangle3d B, List<double> XR, List<double> YR, double EP, double IP, int PA)
        {

            var m_tileSet = new List<Rectangle3d>();

            Plane m_basePlane = new Plane(B.Corner(0), B.Plane.XAxis, B.Plane.YAxis);

            double m_xSum = XR.Sum();
            double m_ySum = YR.Sum();

            double m_padSource = (PA == 0 ? B.Width : B.Height);
            var m_iPadding = IP * m_padSource;
            var m_ePadding = EP * m_padSource;

            double m_availableWidth = B.Width - (2 * m_ePadding) - ((XR.Count - 1) * m_iPadding);
            double m_availableHeight = B.Height - (2 * m_ePadding) - ((YR.Count - 1) * m_iPadding);

            double m_rowVal = 0;

            for (int i = 0; i < YR.Count; i++)
            {
                m_rowVal += (i == 0 ? m_ePadding : m_iPadding);
                double m_nextY = m_rowVal + (YR[i] / m_ySum) * m_availableHeight;

                double m_colVal = 0;
                for (int j = 0; j < XR.Count; j++)
                {
                    m_colVal += (j == 0 ? m_ePadding : m_iPadding);
                    double m_nextX = m_colVal + (XR[j] / m_xSum) * m_availableWidth;

                    Rectangle3d relayRect = new Rectangle3d(m_basePlane,
                      m_basePlane.PointAt(m_colVal, m_rowVal, 0),
                      m_basePlane.PointAt(m_nextX, m_nextY, 0));
                    m_tileSet.Add(new Rectangle3d(new Plane(relayRect.Corner(0), relayRect.Plane.XAxis, relayRect.Plane.YAxis), relayRect.Width, relayRect.Height));

                    m_colVal = m_nextX;
                }

                m_rowVal = m_nextY;
            }

            return m_tileSet;

        }

    }
}
