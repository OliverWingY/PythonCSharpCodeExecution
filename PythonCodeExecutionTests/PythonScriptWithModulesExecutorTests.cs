using Xunit;
using PythonCodeExecution;
using FluentAssertions;

namespace PythonCodeExecutionTests

{
    public class PythonSimpleScriptExecutorTests
    {
        private string LibPath = @"C:\Users\OliverWingYoung\anaconda3\Lib\site-packages";

        [Fact]
        public void Constructor_should_not_throw()
        {
            //arrange
            Action act = () => new PythonScriptWithModulesExecutor();
            //act
            //assert
            act.Should().NotThrow();
        }

        //as it turns out ironpython was dropped by microsoft, and so only archived versions of numpy etc work, recommended to use python.net

        [Fact]
        public void ExecuteScript_should_successfully_import_and_use_numpy_types()
        {
            //arrange
            var moduleName = "numpy";

            var expectedOutput = 5;
            var script = $"A = numpy.array([2,3,4,5] {Environment.NewLine}n = A[3]";
            var underTest = new PythonScriptWithModulesExecutor();
            //act
            var realOutput = underTest.ExecuteScript(script, "n", moduleName, LibPath);
            //assert
            realOutput.Should().Be(expectedOutput);

        } 

    }
}