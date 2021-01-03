/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public interface IControlState<T> 
    {
        T Default { get; set; }
        string ValueName { get; }

        /// <summary>
        /// Get value that in controllableComponent.
        /// </summary>
        /// <returns>Value.</returns>
        T GetValue();

        /// <summary>
        /// Set value that in controllableComponent.
        /// </summary>
        /// <param name="valueIn">Value.</param>
        void SetValue(T valueIn, bool record = true);
    }
}
