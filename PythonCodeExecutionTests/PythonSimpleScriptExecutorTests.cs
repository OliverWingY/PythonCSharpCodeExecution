using Xunit;
using PythonCodeExecution;
using FluentAssertions;

namespace PythonCodeExecutionTests

{
    public class PythonScriptWithModulesExecutorTests
    {
        [Fact]
        public void Constructor_should_not_throw()
        {
            //arrange
            Action act = () => new PythonSimpleScriptExecutor();
            //act
            //assert
            act.Should().NotThrow();
        }

        [Fact]
        public void ExecuteScript_should_get_out_expected_output()
        {
            //arrange
            var expectedOutput = 5.0;            
            var script = "n = float(5)";
            var underTest = new PythonSimpleScriptExecutor();
            //act
            var realOutput = underTest.ExecuteScript(script, "n");
            //assert
            realOutput.Should().Be(expectedOutput);
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
            var underTest = new PythonSimpleScriptExecutor();
            //act
            var realOutput = underTest.ExecuteScript(script, inputName, inputValue, outputName);
            //assert
            realOutput.Should().Be(expectedOutput);
        }

        [Fact]
        public void ExecuteScript_should_correctly_handle_multiple_inputs_and_outputs()
        {
            //arrange
            var expectedOutputs = new Dictionary<string, double>();
            expectedOutputs.Add("C", 10);
            expectedOutputs.Add("D", 24);

            var inputs = new Dictionary<string, double>();
            inputs.Add("A", 4);
            inputs.Add("B", 6);

            var outputs = new List<string> { "C", "D" };
            var script = $"C = A+B {Environment.NewLine}D=A*B";
            var underTest = new PythonSimpleScriptExecutor();
            //act
            var realOutput = underTest.ExecuteScript(script, inputs, outputs);
            //assert
            realOutput.Should().Contain(expectedOutputs);
        }

    }
}