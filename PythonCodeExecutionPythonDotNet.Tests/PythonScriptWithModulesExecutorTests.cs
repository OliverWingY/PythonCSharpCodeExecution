using Xunit;
using PythonCodeExecutionPythonDotNet;
using Python.Runtime;
using ExampleLibrary;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic;

namespace PythonCodeExecutionPythonDotNet.Tests

{
    public class PythonScriptWithModulesExecutorTests
    {
        private PythonScriptWithModulesExecutor? UnderTest;
        private PythonScriptWithModulesExecutor GetUnderTest()
        {
            if (UnderTest is null)
                UnderTest = new PythonScriptWithModulesExecutor();
            return UnderTest;
        }

        private void Cleanup()
        {
            GetUnderTest();
            UnderTest.Dispose();
        }

        [Fact]
        public void Constructor_should_not_throw()
        {
            //arrange
            Action act = () => new PythonScriptWithModulesExecutor();
            //act
            //assert
            act.Should().NotThrow();
            Cleanup();
        }

        [Fact]
        public void ExecuteScript_should_successfully_import_and_use_numpy_types()
        {
            //arrange
            var moduleName = "numpy";

            var expectedOutput = 5.0;
            var script = $"A = numpy.array([2,3,4,5]) {Environment.NewLine}n = A[3].item()";
            var underTest = GetUnderTest();
            //act
            var realOutput = underTest.ExecuteScript(script, "n", moduleName);
            //assert
            expectedOutput.Should().Be(((PyObject)realOutput).ToDouble(System.Globalization.CultureInfo.InvariantCulture));
            Cleanup();
        }

        [Fact]
        public void ExecuteScript_should_successfully_return_numpy_types() //restricting outputs to native python types is probably fine
        {
            //arrange
            var moduleName = "numpy";

            var expectedOutput = new List<int> { 2,3,4,5};
            var script = $"A = numpy.array([2,3,4,5])";
            var underTest = GetUnderTest();
            //act
            object[] realOutput = underTest.ExecuteScriptDynamic(script, "A", moduleName); 
            //assert
            var convertedOutput = realOutput.Cast<PyObject>().ToList().Select(x =>  x.ToInt32(System.Globalization.CultureInfo.InvariantCulture)).ToList();
            convertedOutput.Should().BeEquivalentTo(expectedOutput);
            Cleanup();
        }

        [Fact]
        public void ExecuteScript_should_successfully_use_pandas_dataframe()
        {
            //arrange
            var moduleName = "pandas";
            var moduleAs = "pd";
            var expectedOutput = 5;
            var outputName = "df";
            var script = "d = {'col1': [1, 2], 'col2': [3, 4]}" +
                $"{Environment.NewLine}df = pd.DataFrame(data=d)";
            var underTest = GetUnderTest();
            //act
            var realOuput = underTest.ExecuteScriptDynamic(script, outputName, moduleName, moduleAs);


        }

        [Fact]
        public void ExecuteScript_should_successfully_import_and_use_CSharp_class()
        {
            //arrange
            var underTest = GetUnderTest();
            var moduleName = "clr";
            var outputName = "age";
            var expectedOutput = 20;
            //act
            var script = $"clr.AddReference(\"ExampleLibrary\"); " +
                $"{Environment.NewLine}from ExampleLibrary import Person;" +
                $"{Environment.NewLine}myPerson = Person(\"Tom\", 20)" +
                $"{Environment.NewLine}age = myPerson.Age";
            var realOutput = Convert.ToDouble(underTest.ExecuteScript(script, outputName, moduleName));
            //assert
            realOutput.Should().Be(expectedOutput);
            Cleanup();
        }

        [Fact]
        public void ExecuteScript_should_successfully_call_methods_from_cSharp_class()
        {
            //arrange
            var underTest = GetUnderTest();
            var moduleName = "clr";
            var outputName = "MyBool";
            var expectedOutput = 1;
            //act
            var script = $"clr.AddReference(\"ExampleLibrary\"); " +
                $"{Environment.NewLine}from ExampleLibrary import Club;" +
                $"{Environment.NewLine}myClub = Club()" +
                $"{Environment.NewLine}myClub.ChangeMyBoolToTrue()" +
                $"{Environment.NewLine}if myClub.MyBool:" +
                $"{Environment.NewLine} MyBool = 1" +
                $"{Environment.NewLine}else:" +
                $"{Environment.NewLine} MyBool =0";
            var realOutput = Convert.ToDouble(underTest.ExecuteScript(script, outputName, moduleName));
            //assert
            realOutput.Should().Be(expectedOutput);
            Cleanup();
        }

        [Fact]
        public void Should_be_able_to_access_child_classes_from_parent_without_explicitly_importing_child_class()
        {
            //arrange
            var underTest = GetUnderTest();
            var moduleName = "clr";
            var outputName = "age";
            var expectedOutput = 80;
            //act
            var script = $"clr.AddReference(\"ExampleLibrary\"); " +
                $"{Environment.NewLine}from ExampleLibrary import Club;" +
                $"{Environment.NewLine}myClub = Club()" +
                $"{Environment.NewLine}nancy = myClub.People[1]" +
                $"{Environment.NewLine}age = nancy.Age";
            var realOutput = Convert.ToDouble(underTest.ExecuteScript(script, outputName, moduleName));
            //assert
            realOutput.Should().Be(expectedOutput);
            Cleanup();
        }

        [Fact]
        public void Should_be_able_to_get_csharp_class_as_output()
        {
            //arrange
            var underTest = GetUnderTest();
            var moduleName = "clr";
            var outputName = "myPerson";
            var expectedOutput = new Person("Nancy", 85);
            //act
            var script = $"clr.AddReference(\"ExampleLibrary\"); " +
                $"{Environment.NewLine}from ExampleLibrary import Club;" +
                $"{Environment.NewLine}myClub = Club()" +
                $"{Environment.NewLine}myPerson = myClub.People[1]" +
                $"{Environment.NewLine}myPerson.Age = 85";
            var realOutput = (Person)underTest.ExecuteScript(script, outputName, moduleName);
            //assert
            realOutput.Should().BeEquivalentTo(expectedOutput);
            Cleanup();
        }

        [Fact]
        public void Should_be_able_to_pass_in_instantiated_class_and_modify_from_python_scope()
        {
            //arrange
            var underTest = GetUnderTest();
            var moduleName = "clr";
            var outputName = "age";
            var inputName = "Nancy";
            var inputValue = new Person("Nancy", 20);
            var expectedOutput = 19;
            //act
            var script = $"" +
                $"{Environment.NewLine}Nancy.Age = 19" +
                $"{Environment.NewLine}age = Nancy.Age" +
                $"{Environment.NewLine}Nancy.Age = 18";
            var realOutput = Convert.ToDouble(underTest.ExecuteScript(script, inputName, inputValue, outputName, moduleName));
            realOutput.Should().Be(expectedOutput);
            inputValue.Age.Should().Be(18);
            Cleanup();
        }

        [Fact]
        public void Can_pass_in_dictionary_convert_to_pyDict_then_to_pandas_dataFrame_and_get_back_as_dictionary()
        {
            //arrange
            var underTest = GetUnderTest();
            var moduleNames = new List<string>() { "clr", "numpy", "pandas" };
            Dictionary<string, int[]> expectedOutput = new Dictionary<string, int[]>();
            expectedOutput.Add("col1", new int[2] { 1, 2 });
            expectedOutput.Add("col2", new int[2] { 3, 4 });
            expectedOutput.Add("col3", new int[2] { 5, 6 });
            var outputName = "outputArray"; 
            var script = "d ={}" +
                $"{Environment.NewLine}for k in input.keys():" + 
                $"{Environment.NewLine} d[k]=input[k]" +    
                $"{Environment.NewLine}df = pandas.DataFrame(data=d)" + 
                $"{Environment.NewLine}outputArray = df.to_dict('list')";

            //act
            var output = underTest.ExecuteScriptDynamic(script, "input", expectedOutput.ToPython(), outputName, moduleNames);
            var processedOutput = ConvertPyObjectDictionaryToStringIntArrayDictionary(output);
            //assert
            expectedOutput.Should().BeEquivalentTo(processedOutput);
        }

        [Fact]
        public void Can_pass_in_dictionary_convert_to_pandas_dataFrame_and_get_back_as_dictionary() //
        {
            //arrange
            var underTest = GetUnderTest();
            var moduleNames = new List<string>() { "clr", "numpy", "pandas" };
            Dictionary<string, int[]> expectedOutput = new Dictionary<string, int[]>();
            expectedOutput.Add("col1", new int[2] { 1, 2 });
            expectedOutput.Add("col2", new int[2] { 3, 4 });
            expectedOutput.Add("col3", new int[2] { 5, 6 });
            var bleh = expectedOutput.ToPython();
            var outputName = "outputArray";
            var script = "d =input" +
                $"{Environment.NewLine}df = pandas.DataFrame(data=d)" +
                $"{Environment.NewLine}outputArray = df.to_dict('list')";

            //act
            var output = underTest.ExecuteScriptDynamic(script, "input", expectedOutput.ToPython(), outputName, moduleNames);
            var processedOutput = ConvertPyObjectDictionaryToStringIntArrayDictionary(output);
            //assert
            expectedOutput.Should().BeEquivalentTo(processedOutput);
        }

        [Fact]
        public void Can_create_pandas_dataFrame_and_get_back_as_dictionary()
        {
            //arrange
            var underTest = GetUnderTest();
            var moduleNames = new List<string>() { "clr", "numpy", "pandas"};
            Dictionary<string, int[]> expectedOutput = new Dictionary<string, int[]>();
            expectedOutput.Add("col1", new int[2] { 1, 2 });
            expectedOutput.Add("col2", new int[2] { 3, 4 });
            expectedOutput.Add("col3", new int[2] { 5, 6 });
            dynamic input = expectedOutput.ToPython();
            var outputName = "outputArray";
            var script = "d = dict([('col1',[1,2]), ('col2',[3,4]), ('col3',[5,6])])" +
                $"{Environment.NewLine}df = pandas.DataFrame(data=d)" +
                $"{Environment.NewLine}outputArray = df.to_dict('list')";

            //act
            var output = underTest.ExecuteScriptDynamic(script, "input", input, outputName, moduleNames);
            var processedOutput = ConvertPyObjectDictionaryToStringIntArrayDictionary(output);
            //assert
            expectedOutput.Should().BeEquivalentTo(processedOutput);
        }

        private Dictionary<string, int[]> ConvertPyObjectDictionaryToStringIntArrayDictionary(Dictionary<object, object> dict)
        {           
            var newDict = new Dictionary<string, int[]>();
            foreach (var key in dict.Keys)
            {
                var newKey = key.ToString();
                var newValue = ((object[])dict[key]).Select(x => ((PyInt)x).ToInt32()).ToArray();
                newDict.Add(newKey, newValue);
            }
            return newDict;
        }
    }
}