using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonCodeExecutionPythonDotNet
{
    internal static class PythonEngineStarter 
    {
        private static string pythonPath1 = @"C:\Users\OliverWingYoung\AppData\Local\Programs\Python\Python38";
        private static string py_interpreter = "python38.dll";
        internal static void StartPythonEngine()
        {
            if (!PythonEngine.IsInitialized)
            {
                Runtime.PythonDLL = py_interpreter;
                string pathToPython = pythonPath1;
                string path = pathToPython + ";" + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("PYTHONHOME", pathToPython, EnvironmentVariableTarget.Process);
                
                PythonEngine.Initialize();
                PyObjectConversions.RegisterDecoder(new PyDictionaryConverter());
                PyObjectConversions.RegisterEncoder(new PyDictionaryConverter());
            }
        }

        internal static void Dispose()
        {
            PythonEngine.Shutdown();
        }
    }
}
