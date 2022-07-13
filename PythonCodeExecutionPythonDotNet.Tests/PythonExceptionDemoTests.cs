using Xunit;
using PythonCodeExecutionPythonDotNet;
using ExampleLibrary;
using FluentAssertions;

namespace PythonCodeExecutionPythonDotNet.Tests
{
    public class PythonExceptionDemoTests
    {
        private PythonExceptionDemo? UnderTest;
        private PythonExceptionDemo GetUnderTest()
        {
            if (UnderTest is null)
                UnderTest = new PythonExceptionDemo();
            return UnderTest;
        }

        private void Cleanup()
        {
            GetUnderTest();
            UnderTest.Dispose();
        }

        [Fact]
        public void ExecuteScript_Should_return_fail_and_exception_message_Given_bad_script()
        {
            //arrange
            var underTest = GetUnderTest();
            var outputName = "A";
            //act
            var script = $"A=3/0";
            var realOutput = underTest.ExecuteScript(script, outputName);
            //assert
            realOutput.IsSuccess.Should().BeFalse();
            realOutput.Error.Should().Be("Python script failed with exception:division by zero");
            Cleanup();
        }

        [Fact]
        public void ExecuteScript_Should_get_ouput_Given_good_script()
        {
            //arrange
            var underTest = GetUnderTest();
            var outputName = "A";
            var expectedOutput = 15.0;
            //act
            var script = $"A=10+5";
            var realOutput = underTest.ExecuteScript(script, outputName);
            //assert
            realOutput.IsSuccess.Should().BeTrue();
            Convert.ToDouble(realOutput.Value).Should().Be(expectedOutput);
            Cleanup();
        }

    }
}
