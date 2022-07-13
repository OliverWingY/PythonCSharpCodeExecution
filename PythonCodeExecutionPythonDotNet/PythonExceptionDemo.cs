using CSharpFunctionalExtensions;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonCodeExecutionPythonDotNet
{
    public class PythonExceptionDemo
    {
        private string pythonPath1 = @"C:\Users\OliverWingYoung\AppData\Local\Programs\Python\Python38";
        private string py_interpreter = "python38.dll";
        public PythonExceptionDemo()
        {
            PythonEngineStarter.StartPythonEngine();
        }

        public void Dispose()
        {

        }

        public Result WithPythonExceptionPolicy(Action action)
        {
            try
            {
                action();
            }
            catch (PythonException e)
            {
                return Result.Failure($"Python script failed with exception:{e.Message}");
            };
            return Result.Success();
        }

        public Result<object> ExecuteScript(string script, string outputName)
        {
            using (Py.GIL())
            {
                var scope = Py.CreateScope();
                return WithPythonExceptionPolicy(() => scope.Exec(script)).Bind(() =>
                {
                    object output = scope.Get<object>(outputName);
                    scope.Dispose();
                    return Result.Success(output);
                });           
 
            }
        }
    }
}
