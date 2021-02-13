using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhino.NodeInCode;
using Grasshopper.Kernel;

namespace TestProject
{
    class FunctionFinder
    {
        #region FunctionDictionary Property && its Method
        private ConcurrentDictionary<Guid, ComponentFunctionInfo> _funcDictionary;

        public ConcurrentDictionary<Guid, ComponentFunctionInfo> FunctionDictionary
        {
            get 
            {
                //check if _funcDictionary if null.
                _funcDictionary = _funcDictionary ?? GetDictionary();

                return _funcDictionary; 
            }
        }

        /// <summary>
        /// Get the Guid & FunctionInfo Dictionary.
        /// </summary>
        /// <returns>the dictionary</returns>
        private ConcurrentDictionary<Guid, ComponentFunctionInfo> GetDictionary()
        {
            //find the m_guids private field.
            IEnumerable<FieldInfo> infos = typeof(NodeInCodeTable).GetRuntimeFields().Where((info) => info.Name.Contains("m_guids"));

            // check whether to find the info.
            if (infos.Count() != 1) return null;

            //return the right dictionary;
            return infos.ElementAt(0).GetValue(Components.NodeInCodeFunctions) as ConcurrentDictionary<Guid, ComponentFunctionInfo>;
        }
        #endregion


        /// <summary>
        /// Find the component's function by component's guid.
        /// </summary>
        /// <param name="componentguid">component's guid.</param>
        /// <returns>The Function</returns>
        public ComponentFunctionInfo FindComponentFunction(Guid componentguid)
        {
            return FunctionDictionary[componentguid];
        }

        /// <summary>
        /// Find the component's function by component itself.
        /// </summary>
        /// <param name="object">documentObject</param>
        /// <returns>The Function</returns>
        public ComponentFunctionInfo FindComponentFunction(IGH_DocumentObject @object)
        {
            return FindComponentFunction(@object.ComponentGuid);
        }



    }
}
