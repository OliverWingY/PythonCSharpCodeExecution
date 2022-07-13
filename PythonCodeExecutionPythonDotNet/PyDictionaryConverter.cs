using System;
using Python.Runtime;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonCodeExecutionPythonDotNet
{
    internal class PyDictionaryConverter : IPyObjectDecoder, IPyObjectEncoder
    {
        public bool CanDecode(PyType objectType, Type targetType)
        {
            if (objectType.Name == "dict")
                return true;
            return false;
        }

        public bool TryDecode<IDictionary>(PyObject pyObj, out IDictionary? value)
        {
            try
            {
                var pyDict = new PyDict(pyObj);
                value = (IDictionary)ToClr(pyDict);
                return true;
            }
            catch
            {
                throw new InvalidCastException("Failed to cast Python Dictionary to Dictionary");
            }
        }

        private static IDictionary<object, object> ToClr(PyDict pyDict)
        {
            var dict = new Dictionary<object, object>();
            foreach (PyObject key in pyDict.Keys())
            {
                var clrKey = key.AsManagedObject(typeof(object));
                var clrValue = pyDict[key].AsManagedObject(typeof(object));
                dict.Add(clrKey, clrValue); 
            }
            return dict;
        }

        public bool CanEncode(Type type) //will only try to encode if not already a pyObject
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return true;
            return false;
        }

        public PyObject? TryEncode(object value)
        {
            dynamic dict = value;
            var newPyDict = new PyDict();
            foreach (var key in dict.Keys)
            {
                var pyKey = ((object)key).ToPython();
                var pyValue = ((object)dict[key]).ToPython();
                newPyDict.SetItem(pyKey, pyValue);
            }
            return newPyDict;
        }
    }
}
