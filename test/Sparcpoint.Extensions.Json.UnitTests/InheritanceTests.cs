using System.Text.Json;

namespace Sparcpoint.Extensions.Json.UnitTests;

public class InheritanceTests
{
    const string BASE_VALUE = "base-value";
    const string DERIVED_VALUE = "derived-value";

    public InheritanceTests()
    {
        Options = new JsonSerializerOptions();
    }

    public JsonSerializerOptions Options { get; }

    [Fact]
    public void ConfirmInheritanceNormalOperation()
    {
        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE), (nameof(BaseClass.BaseProperty), BASE_VALUE));
    }

    [Fact]
    public void GivenInheritedTypesIncluded_WhenBaseNameChanged_ReflectedInDerivedType()
    {
        const string PROP_NAME = "new-base";

        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Name(PROP_NAME), true);
        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE), (PROP_NAME, BASE_VALUE));
    }

    [Fact]
    public void GivenInheritedTypesNotIncluded_WhenBaseNameChanged_NotChangedInDerivedType()
    {
        const string PROP_NAME = "new-base";

        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Name(PROP_NAME), false);
        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE), (nameof(BaseClass.BaseProperty), BASE_VALUE));
    }

    [Fact]
    public void GivenInheritedTypesIncluded_WhenBaseIgnored_ReflectedInDerivedType()
    {
        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Ignore(), true);
        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE));
    }

    [Fact]
    public void GivenInheritedTypesNotIncluded_WhenBaseIgnored_NotChangedInDerivedType()
    {
        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Ignore(), false);
        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE), (nameof(BaseClass.BaseProperty), BASE_VALUE));
    }

    [Fact]
    public void GivenInheritedTypesIncluded_WhenBaseOrderLowered_ReflectedInDerivedType()
    {
        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Order(-100), true);
        AssertJson((nameof(BaseClass.BaseProperty), BASE_VALUE), (nameof(DerivedClass.DerivedProperty), DERIVED_VALUE));
    }

    [Fact]
    public void GivenInheritedTypesNotIncluded_WhenBaseOrderLowered_NotChangedInDerivedType()
    {
        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Order(-100), false);
        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE), (nameof(BaseClass.BaseProperty), BASE_VALUE));
    }

    [Fact]
    public void DerivedNameChangeOverridesBaseNameChange()
    {
        const string BASE_NAME = "base-name-changed";
        const string DERIVED_NAME = "derived-name-changed";

        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Name(BASE_NAME), true);
        Options.WithHigherPriority<DerivedClass>(b => b.Property(t => t.BaseProperty).Name(DERIVED_NAME), true);

        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE), (DERIVED_NAME, BASE_VALUE));
    }

    [Fact(Skip = "Bigger changes are needed to ordering for this to work")]
    public void DerivedNameChangeOverridesBaseNameChange_ReversedOrder()
    {
        const string BASE_NAME = "base-name-changed";
        const string DERIVED_NAME = "derived-name-changed";

        Options.WithHigherPriority<DerivedClass>(b => b.Property(t => t.BaseProperty).Name(DERIVED_NAME), true);
        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Name(BASE_NAME), true);

        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE), (DERIVED_NAME, BASE_VALUE));
    }

    [Fact]
    public void HigherPriorityBaseClassNotInherited_DoesNotOverrideDerivedNotInherited()
    {
        const string BASE_NAME = "base-name-changed";
        const string DERIVED_NAME = "derived-name-changed";

        Options.WithHigherPriority<DerivedClass>(b => b.Property(t => t.BaseProperty).Name(DERIVED_NAME), false);
        Options.WithHigherPriority<BaseClass>(b => b.Property(t => t.BaseProperty).Name(BASE_NAME), false);

        AssertJson((nameof(DerivedClass.DerivedProperty), DERIVED_VALUE), (DERIVED_NAME, BASE_VALUE));
    }

    private void AssertJson(params (string Name, string Value)[] expectedProps)
    {
        var actual = JsonSerializer.Serialize(new DerivedClass(), Options);

        List<string> props = new();
        foreach (var kv in expectedProps)
        {
            props.Add($"\"{kv.Name}\":\"{kv.Value}\"");
        }

        var expected = "{" + string.Join(",", props) + "}";
        Assert.Equal(expected, actual);
    }

    public class BaseClass
    {
        public string BaseProperty { get; set; } = BASE_VALUE;
    }

    public class DerivedClass : BaseClass
    {
        public string DerivedProperty { get; set; } = DERIVED_VALUE;
    }
}

