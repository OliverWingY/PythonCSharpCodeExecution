using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonCodeExecutionPythonDotNet
{
    public class PythonScriptWithModulesExecutor
    {
        private string pythonPath1 = @"C:\Users\OliverWingYoung\AppData\Local\Programs\Python\Python38";
        private string py_interpreter = "python38.dll";
        public PythonScriptWithModulesExecutor()
        {
            if (!PythonEngine.IsInitialized)
            {
                Runtime.PythonDLL = py_interpreter;
                string pathToPython = pythonPath1;
                string path = pathToPython + ";" + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("PYTHONHOME", pathToPython, EnvironmentVariableTarget.Process);

                PythonEngine.Initialize();
            }
        }

        public void Dispose()
        {
            PythonEngine.Shutdown();
        }

        public object ExecuteScript(string script, string outputName, string moduleName)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                scope.Import(moduleName);
                scope.Exec(script);
                object output = scope.Get<object>(outputName);
                scope.Dispose();
                return output;
            }
        }

        public object ExecuteScript(string script, string outputName, List<string> moduleNames)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                ImportModules(scope, moduleNames);
                scope.Exec(script);
                object output = scope.Get<object>(outputName);
                scope.Dispose();
                return output;
            }            
        }

        public object ExecuteScript(string script, string inputName, object inputValue, string outputName, string moduleName)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                scope.Import(moduleName);
                scope.Set(inputName, inputValue);
                scope.Exec(script);
                object output = scope.Get<object>(outputName);
                scope.Dispose();
                return output;
            }
        }

        private void ImportModules(PyModule scope, List<string> moduleNames)
        {
            foreach (var moduleName in moduleNames)
            {
                scope.Import(moduleName);
            }
        }

    }
}
