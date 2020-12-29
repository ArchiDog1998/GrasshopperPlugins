/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public class Iconable
    {
		private int m_icon_index;
		private int m_icon_index_false;
		private int m_icon_index_locked;
		private Bitmap Icon;
		protected Bitmap Icon_24x24_True
		{
			get
			{
				if (m_icon_index < 0)
				{
					Bitmap bitmap = Icon;
					if (bitmap != null)
					{
						switch (bitmap.PixelFormat)
						{
							default:
								bitmap = GH_IconTable.To32BppArgb(bitmap);
								break;
							case PixelFormat.Format24bppRgb:
							case PixelFormat.Format32bppRgb:
							case PixelFormat.Format32bppPArgb:
							case PixelFormat.Format32bppArgb:
								break;
						}
					}
					m_icon_index = GH_IconTable.RegisterIcon(bitmap);
				}
				return GH_IconTable.Icon(m_icon_index);
			}
		}
		protected Bitmap Icon_24x24_Locked
		{
			get
			{
				if (m_icon_index_locked < 0)
				{
					Bitmap bitmap = Icon_24x24_True;
					if (bitmap != null)
					{
						bitmap = (Bitmap)Icon_24x24_True.Clone();
						GH_MemoryBitmap gH_MemoryBitmap = new GH_MemoryBitmap(bitmap);
						gH_MemoryBitmap.Filter_GreyScale();
						gH_MemoryBitmap.Filter_Dullify();
						gH_MemoryBitmap.Filter_Blur(3, 1);
						gH_MemoryBitmap.Release(includeChanges: true);
					}
					m_icon_index_locked = GH_IconTable.RegisterIcon(bitmap);
				}
				return GH_IconTable.Icon(m_icon_index_locked);
			}
		}

		protected Bitmap Icon_24x24_False
		{
			get
			{
				if (m_icon_index_false < 0)
				{
					Bitmap bitmap = Icon_24x24_True;
					if (bitmap != null)
					{
						bitmap = (Bitmap)Icon_24x24_True.Clone();
						GH_MemoryBitmap gH_MemoryBitmap = new GH_MemoryBitmap(bitmap);
						gH_MemoryBitmap.Filter_GreyScale(0.8);
						gH_MemoryBitmap.Filter_Dullify();
						gH_MemoryBitmap.Release(includeChanges: true);
					}
					m_icon_index_false = GH_IconTable.RegisterIcon(bitmap);
				}
				return GH_IconTable.Icon(m_icon_index_false);
			}
		}

		public Bitmap GetIcon(bool enable, bool on)
        {
            if (enable)
            {
				return on ? this.Icon_24x24_True : this.Icon_24x24_False;
            }
            else
            {
				return this.Icon_24x24_Locked;
            }
        }

		public Iconable(Bitmap icon)
        {
			this.Icon = icon;
			this.m_icon_index = -1;
			this.m_icon_index_false = -1;
			this.m_icon_index_locked = -1;
		}
	}
}
