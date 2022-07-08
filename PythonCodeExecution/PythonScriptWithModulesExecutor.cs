using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonCodeExecution
{
    public class PythonScriptWithModulesExecutor
    {
        private ScriptEngine engine;
        
        public PythonScriptWithModulesExecutor()
        {
            InitialsePythonScripting();
        }
        private void InitialsePythonScripting()
        {
            engine = Python.CreateEngine();
        }

        public double ExecuteScript(string script, string outputName, string moduleName, string modulesPath)
        {
            var paths = engine.GetSearchPaths();
            paths.Add(modulesPath);
            engine.SetSearchPaths(paths);
            var scope = engine.CreateScope();
            engine.ImportModule(moduleName);
            engine.Execute(script, scope);
            
            double output = scope.GetVariable(outputName);
            return output;
        }
    }
}
