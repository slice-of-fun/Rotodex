using NetArchTest.Rules;
using Xunit;

namespace RotoDex.Integration.Tests
{
    public class ArchitectureTests
    {
        [Fact]
        public void Applications_ShouldNotReference_Adapter()
        {
            var result = Types.InAssembly(typeof(Desktop.App).Assembly)
                .ShouldNot()
                .HaveDependencyOn("RotoDex.Adapter")
                .GetResult();

            Assert.True(result.IsSuccessful, "Applications must not reference the Adapter layer directly.");
        }

        [Fact]
        public void Applications_ShouldNotReference_CoreEngine()
        {
            var result = Types.InAssembly(typeof(Desktop.App).Assembly)
                .ShouldNot()
                .HaveDependencyOn("Roto.Core")
                .GetResult();

            Assert.True(result.IsSuccessful, "Applications must not reference the upstream Core engine.");
        }

        [Fact]
        public void CoreLayer_ShouldNotReference_CoreEngine()
        {
            var result = Types.InAssembly(typeof(RotoDex.Core.Resources.ResourceManager).Assembly)
                .ShouldNot()
                .HaveDependencyOn("Roto.Core")
                .GetResult();

            Assert.True(result.IsSuccessful, "The RotoDex.Core layer must not reference Roto.Core directly. Only the Adapter can do that.");
        }
    }
}
