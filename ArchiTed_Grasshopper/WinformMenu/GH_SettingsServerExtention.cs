/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper
{
    public static class GH_SettingsServerExtention
    {
        #region Guid
        public static Guid GetValue(this GH_SettingsServer server, string key, Guid @default)
        {
            bool isSuccess = true;

            //Find bytes.
            byte[] bytes = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                //get byte.
                byte _byte = server.GetValue(key + i.ToString("D2"), default(byte));

                //Check whether succeed.
                if (_byte == default(byte))
                {
                    isSuccess = false;
                    break;
                }
                else
                {
                    bytes[i] = _byte;
                }
            }

            return isSuccess ? new Guid(bytes) : (Guid)@default;
        }

        public static void SetValue(this GH_SettingsServer server, string key, Guid value)
        {
            //Get all bytes.
            byte[] bytes = ((Guid)value).ToByteArray();

            //Save each bytes.
            for (int i = 0; i < 16; i++)
            {
                server.SetValue(key + i.ToString("D2"), bytes[i]);
            }
        }
        #endregion

        #region IEnumerable<Guid>
        public static IEnumerable<Guid> GetValue(this GH_SettingsServer server, string key, IEnumerable<Guid> @default)
        {
            //Get count;
            int count = server.GetValue(key + "Count", 0);

            Guid[] result = new Guid[count];
            for (int i = 0; i < count; i++)
            {
                //Get every guid.
                Guid temp = server.GetValue(key + i.ToString("D10"), Guid.Empty);
                if (temp == Guid.Empty) throw new Exception(key + i.ToString("D10") + "is not found!");
                result[i] = temp;
            }
            return result;
        }

        public static void SetValue(this GH_SettingsServer server, string key, IEnumerable<Guid> value)
        {
            int count = value.Count();
            server.SetValue(key + "Count", count);
            for (int i = 0; i < count; i++)
            {
                server.SetValue(key + i.ToString("D10"), value.ElementAt(i));
            }
        }
        #endregion
    }
}
