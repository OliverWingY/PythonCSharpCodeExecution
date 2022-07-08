using IronPython;
using System;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace PythonCodeExecution
{
    public class PythonSimpleScriptExecutor
    {
        private ScriptEngine engine;

        public PythonSimpleScriptExecutor()
        {
            engine = Python.CreateEngine();
        }

        public double ExecuteScript(string script, string outputName)
        {
            var scope = engine.CreateScope();
            engine.Execute(script, scope);

            double output = scope.GetVariable(outputName);            
            return output;
        }

        public double ExecuteScript(string script, string inputName, double inputValue, string outputName)
        {
            var scope = engine.CreateScope();
            scope.SetVariable(inputName, inputValue);
            engine.Execute(script, scope);

            double output = scope.GetVariable(outputName);
            return output;
        }

        public Dictionary<string, double> ExecuteScript(string script, Dictionary<string,double> inputs, List<string> outputs)
        {
            var scope = engine.CreateScope();
            SetScriptInputs(scope, inputs);
            engine.Execute(script, scope);

            var output = GetScripOutputs(scope, outputs);
            return output;

        }
        private void SetScriptInputs(ScriptScope scope, Dictionary<string, double> inputs)
        {
            foreach (var input in inputs)
            {
                scope.SetVariable(input.Key, input.Value);
            }
        }

        private Dictionary<string, double> GetScripOutputs(ScriptScope scope, List<string> outputs)
        {
            var foundOutputs = new Dictionary<string, double>();
            foreach (var output in outputs)
            {
                if (scope.TryGetVariable(output, out double outputValue))
                    foundOutputs.Add(output, outputValue);
            }
            return foundOutputs;
        }

    }
}