/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.Unsafe
{
    public static class UnsafeHelper
    {
        public static bool Exchange(Type targetType, string targetMethod, Type injectType, string injectMethod, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, Binder binder = null, CallingConventions callConvention = CallingConventions.Any, Type[] types = null, ParameterModifier[] modifiers = null)
        {
            if (types == null)
            {
                types = Type.EmptyTypes;
            }
            MethodInfo tarMethod = targetType.GetMethod(targetMethod, bindingAttr, binder, callConvention, types, modifiers);
            MethodInfo injMethod = injectType.GetMethod(injectMethod, bindingAttr, binder, callConvention, types, modifiers);

            return ExchangeMethod(tarMethod, injMethod);
        }

        public static bool ExchangeMethod(MethodInfo targetMethod, MethodInfo injectMethod)
        {
            if (targetMethod == null || injectMethod == null)
            {
                return false;
            }
            RuntimeHelpers.PrepareMethod(targetMethod.MethodHandle);
            RuntimeHelpers.PrepareMethod(injectMethod.MethodHandle);
            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    int* tar = (int*)targetMethod.MethodHandle.Value.ToPointer() + 2;
                    int* inj = (int*)injectMethod.MethodHandle.Value.ToPointer() + 2;
                    var relay = *tar;
                    *tar = *inj;
                    *inj = relay;
                }
                else
                {
                    long* tar = (long*)targetMethod.MethodHandle.Value.ToPointer() + 1;
                    long* inj = (long*)injectMethod.MethodHandle.Value.ToPointer() + 1;
                    var relay = *tar;
                    *tar = *inj;
                    *inj = relay;
                }
            }
            return true;
        }
    }
}
