using Python;
using Python.Runtime;
using System;

namespace PythonCodeExecutionPythonDotNet
{
    public class PythonSimpleScriptExecutor : IDisposable
    {
        private string pythonPath1 = @"C:\Users\OliverWingYoung\AppData\Local\Programs\Python\Python38";
        private string py_interpreter = "python38.dll";
        public PythonSimpleScriptExecutor()
        {                 
            if (!PythonEngine.IsInitialized)
            {
                Runtime.PythonDLL = py_interpreter;
                string pathToPython = pythonPath1;
                string path = pathToPython + ";" +
                Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("PYTHONHOME", pathToPython, EnvironmentVariableTarget.Process);

                PythonEngine.Initialize();
            }                      
        }

        public void Dispose()
        {
            PythonEngine.Shutdown();
        }

        public object ExecuteScript(string script, string outputName)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                scope.Exec(script);
                object output = scope.Get(outputName).ToDouble(System.Globalization.CultureInfo.InvariantCulture);
                return output;
            }
        }

        public object ExecuteScript(string script, string inputName, object inputValue, string outputName)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                scope.Set(inputName, inputValue);
                scope.Exec(script);

                object output = scope.Get(outputName);
                return output;
            }
        }

        public Dictionary<string, object> ExecuteScript(string script, Dictionary<string, object> inputs, List<string> outputs)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                SetScriptInputs(scope, inputs);
                scope.Exec(script);

                var output = GetScripOutputs(scope, outputs);
                return output;
            }

        }
        private void SetScriptInputs(PyModule scope, Dictionary<string, object> inputs)
        {
            foreach (var input in inputs)
            {
                scope.Set(input.Key, input.Value);
            }
        }

        private Dictionary<string, object> GetScripOutputs(PyModule scope, List<string> outputs)
        {
            var foundOutputs = new Dictionary<string, object>();
            foreach (var output in outputs)
            {
                foundOutputs.Add(output, scope.Get(output).ToDouble(System.Globalization.CultureInfo.InvariantCulture));
            }
            return foundOutputs;
        }

    }
}