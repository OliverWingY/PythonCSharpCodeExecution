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
        
        public PythonScriptWithModulesExecutor()
        {
            PythonEngineStarter.StartPythonEngine();
        }

        public void Dispose()
        {
            PythonEngineStarter.Dispose();
        }

        public object ExecuteScript(string script, string outputName, string moduleName, string? moduleAs = null)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                scope.Import(moduleName, moduleAs);
                scope.Exec(script);
                object output = scope.Get<object>(outputName);
                scope.Dispose();
                return output;
            }
        }

        public dynamic ExecuteScriptDynamic(string script, string outputName, string moduleName, string? moduleAs = null)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                scope.Import(moduleName, moduleAs);
                scope.Exec(script);
                PyObject output = scope.Get(outputName);
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

        public dynamic ExecuteScriptDynamic(string script, string outputName, List<string> moduleNames)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                ImportModules(scope, moduleNames);
                scope.Exec(script);
                dynamic output = scope.Get<dynamic>(outputName);
                scope.Dispose();
                return output;
            }
        }

        public object ExecuteScript(string script, string inputName, object inputValue, string outputName, string moduleName, string? moduleAs = null)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                scope.Import(moduleName, moduleAs);
                scope.Set(inputName, inputValue);
                scope.Exec(script);
                object output = scope.Get<object>(outputName);
                scope.Dispose();
                return output;
            }
        }

        public dynamic ExecuteScriptDynamic(string script, string inputName, object inputValue, string outputName, List<string> moduleNames)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                ImportModules(scope, moduleNames);
                scope.Set(inputName, inputValue);
                scope.Exec(script);
                dynamic output = scope.Get<dynamic>(outputName);
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
