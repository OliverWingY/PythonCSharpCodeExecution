using Xunit;
using PythonCodeExecutionPythonDotNet;
using ExampleLibrary;
using FluentAssertions;

namespace PythonCodeExecutionPythonDotNet.Tests

{
    public class PythonSimpleScriptExecutorTests
    {
        private PythonSimpleScriptExecutor UnderTest;
        private PythonSimpleScriptExecutor GetUnderTest()
        {
            if (UnderTest is null)
                UnderTest = new PythonSimpleScriptExecutor();
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
            Action act = () => new PythonSimpleScriptExecutor();
            //act
            //assert
            act.Should().NotThrow();
            Cleanup();
        }

        [Fact]
        public void ExecuteScript_should_get_out_expected_output()
        {
            //arrange
            var expectedOutput = 5.0;
            var script = "n = float(5)";
            var underTest = GetUnderTest();
            //act
            var realOutput = underTest.ExecuteScript(script, "n");
            //assert
            realOutput.Should().Be(expectedOutput);
            Cleanup();
        }

        [Fact]
        public void ExecuteScript_should_pass_in_correct_input()
        {
            //arrange
            var expectedOutput = 5.0;
            var script = "B = A+1";
            var inputName = "A";
            var outputName = "B";
            var inputValue = 4.0;
            var underTest = GetUnderTest();
            //act
            var realOutput = Convert.ToDouble(underTest.ExecuteScript(script, inputName, inputValue, outputName));
            //assert
            realOutput.Should().Be(expectedOutput);
            Cleanup();
        }

        [Fact]
        public void ExecuteScript_should_correctly_handle_multiple_inputs_and_outputs()
        {
            //arrange
            var expectedOutputs = new Dictionary<string, object>();
            expectedOutputs.Add("C", 10);
            expectedOutputs.Add("D", 24);

            var inputs = new Dictionary<string, object>();
            inputs.Add("A", 4);
            inputs.Add("B", 6);

            var outputs = new List<string> { "C", "D" };
            var script = $"C = A+B " +
                $"{Environment.NewLine}D=A*B";
            var underTest = GetUnderTest();
            //act
            var realOutput = underTest.ExecuteScript(script, inputs, outputs);
            //assert
            realOutput.Should().Contain(expectedOutputs);

            Cleanup();
        }

        [Fact]
        public void Should_be_able_to_pass_in_instantiated_class_and_modify_from_python_scope_without_importing_clr()
        {
            //arrange
            var underTest = GetUnderTest();
            var outputName = "age";
            var inputName = "Nancy";
            var inputValue = new Person("Nancy", 20);
            var expectedOutput = 19;
            //act
            var script = $"" +
                $"{Environment.NewLine}Nancy.Age = 19" +
                $"{Environment.NewLine}age = Nancy.Age" +
                $"{Environment.NewLine}Nancy.Age = 18";
            //assert
            var realOutput = Convert.ToDouble(underTest.ExecuteScript(script, inputName, inputValue, outputName));
            realOutput.Should().Be(expectedOutput);
            inputValue.Age.Should().Be(18);
            Cleanup();
        }

    }
}