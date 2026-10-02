using System.Reflection;

using BaseRz.Core.Utilities;

namespace BaseRz.Tests.Utilities;

public class CssTests
{
    [Fact]
    public void Join_SkipsNullAndWhitespace()
    {
        Assert.Equal("a b c", Css.Join("a", null, "", "   ", "b", "\t", "c"));
        Assert.Equal(string.Empty, Css.Join(null, " "));
        Assert.Equal(string.Empty, Css.Join());
    }

    [Fact]
    public void CallerClass_ReadsClass_RemovesNothingFromSource()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "caller",
            ["id"] = "x",
        };

        Assert.Equal("caller", Css.CallerClass(attributes));
        Assert.Equal(2, attributes.Count);
        Assert.Equal("caller", attributes["class"]);

        Assert.Null(Css.CallerClass(null));
        Assert.Null(Css.CallerClass(new Dictionary<string, object> { ["class"] = "  " }));
        Assert.Null(Css.CallerClass(new Dictionary<string, object> { ["class"] = 42 }));
        Assert.Null(Css.CallerClass(new Dictionary<string, object> { ["id"] = "x" }));
    }

    [Fact]
    public void AttributesWithoutClass_ReturnsCopyWithoutClass()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "caller",
            ["id"] = "x",
            ["data-test"] = "y",
        };

        var copy = Css.AttributesWithoutClass(attributes);

        Assert.NotNull(copy);
        Assert.NotSame(attributes, copy);
        Assert.False(copy.ContainsKey("class"));
        Assert.Equal("x", copy["id"]);
        Assert.Equal("y", copy["data-test"]);
        Assert.Equal(3, attributes.Count);
        Assert.Null(Css.AttributesWithoutClass(null));
    }

    [Fact]
    public void PublicApi_IsJoinCallerClassAttributesWithoutClass()
    {
        var methods = typeof(Css)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(static method => method.Name)
            .Order()
            .ToArray();

        Assert.Equal(["AttributesWithoutClass", "CallerClass", "Join"], methods);
        Assert.Equal("BaseRz.Core.Utilities", typeof(Css).Namespace);
        Assert.True(typeof(Css).IsAbstract && typeof(Css).IsSealed);
    }
}
