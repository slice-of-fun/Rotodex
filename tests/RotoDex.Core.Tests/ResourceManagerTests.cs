using System.IO;
using RotoDex.Core.Resources;

namespace RotoDex.Core.Tests;

public class ResourceManagerTests
{
    [Fact]
    public void Initialize_LoadsJsonFiles_Successfully()
    {
        // Arrange
        // Note: In an actual CI environment, the paths might differ.
        // For local integration testing, we point directly to the project's root resources directory.
        var solutionDir = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent?.FullName;
        var resourceDir = Path.Combine(solutionDir ?? "", "resources");

        // Act
        ResourceManager.Initialize(resourceDir);

        // Assert
        Assert.True(ResourceManager.IsInitialized);
        Assert.NotEmpty(ResourceManager.Encounters);
        Assert.NotEmpty(ResourceManager.Learnsets);
        Assert.NotEmpty(ResourceManager.PersonalData);
    }
}
