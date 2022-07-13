using Xunit;
using PythonCodeExecutionPythonDotNet;
using ExampleLibrary;
using FluentAssertions;

namespace PythonCodeExecutionPythonDotNet.Tests
{
    public class EmbeddedPythonDemoTests
    {
        private EmbeddedPythonDemo UnderTest;
        private EmbeddedPythonDemo GetUnderTest()
        {
            if (UnderTest is null)
                UnderTest = new EmbeddedPythonDemo();
            return UnderTest;
        }

        private void Cleanup()
        {
            GetUnderTest();
            UnderTest.Dispose();
        }

        [Fact]
        public void RunMyPandasCode_should_work()
        {
            var undertest = GetUnderTest();
            undertest.RunMyPandasCode();
            Cleanup();
        }
    }
}
