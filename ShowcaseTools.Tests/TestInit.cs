/*  
    Referenced by https://github.com/tmakin/RhinoCommonUnitTesting/blob/master/Src/RhinoPlugin.Tests/TestInit.cs
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ShowcaseTools.Tests
{
    [TestClass]
    public static class TestInit
    {
        static bool initialized = false;

        // Set path to rhino system directory
        static readonly string rhinoSystemDir = @"E:\Software\Install\Mcneel\Rhino7\System";

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            if (initialized)
            {
                throw new InvalidOperationException("AssemblyInitialize should only be called once");
            }
            initialized = true;

            context.WriteLine("Assembly init started");

            // Ensure we are 64 bit
            Assert.IsTrue(Environment.Is64BitProcess, "Tests must be run as x64");

            // Make Sure the path to rhino system directory
            Assert.IsTrue(System.IO.Directory.Exists(rhinoSystemDir), "Rhino system dir not found: {0}", rhinoSystemDir);

            // Add rhino system directory to path (for RhinoLibrary.dll)
            string envPath = Environment.GetEnvironmentVariable("path");
            Environment.SetEnvironmentVariable("path", envPath + ";" + rhinoSystemDir);

            // Add hook for .Net assmbly resolve (for RhinoCommmon.dll)
            AppDomain.CurrentDomain.AssemblyResolve += ResolveRhinoCommon;

            // Start headless Rhino process
            LaunchInProcess(0, 0);
        }

        private static Assembly ResolveRhinoCommon(object sender, ResolveEventArgs args)
        {
            var name = args.Name;

            if (!name.StartsWith("RhinoCommon"))
            {
                return null;
            }

            var path = System.IO.Path.Combine(rhinoSystemDir, "RhinoCommon.dll");
            return Assembly.LoadFrom(path);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // Shotdown the rhino process at the end of the test run
            ExitInProcess();
        }

        [DllImport("RhinoLibrary.dll")]
        internal static extern int LaunchInProcess(int reserved1, int reserved2);

        [DllImport("RhinoLibrary.dll")]
        internal static extern int ExitInProcess();
    }
}


