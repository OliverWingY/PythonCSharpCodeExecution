using Xunit;
using PythonCodeExecutionPythonDotNet;
using ExampleLibrary;
using FluentAssertions;

namespace PythonCodeExecutionTests

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

            var expectedOutput = 5;
            var script = $"A = numpy.array([2,3,4,5]) {Environment.NewLine}n = A[3]";
            var underTest = GetUnderTest();
            //act
            var realOutput = Convert.ToDouble(underTest.ExecuteScript(script, "n", moduleName));
            //assert
            realOutput.Should().Be(expectedOutput);
            Cleanup();
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
    }
}