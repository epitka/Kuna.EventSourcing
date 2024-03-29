using DeepEqual.Syntax;
using Kuna.Utilities.Extensions;

namespace Kuna.Utilities.Tests.Extensions;

public class ObjectExtensionsTests
{
    private class TestObj
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = Guid.NewGuid().ToString();
    }

    [Fact]
    public void DeepClone_Should_Clone_Private_Members()
    {
        var obj = new TestObj();
        var cloned = obj.DeepClone();

        cloned.ShouldDeepEqual(obj);
    }
}
