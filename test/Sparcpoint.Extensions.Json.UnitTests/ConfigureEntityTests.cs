using System.Reflection;
using System.Text.Json;

namespace Sparcpoint.Extensions.Json.UnitTests;

public class ConfigureEntityTests
{
    public JsonSerializerOptions Options { get; }

    public ConfigureEntityTests()
    {
        Options = new JsonSerializerOptions();
    }

    [Fact]
    public void CanConfigureClass()
    {
        Options.WithHigherPriority<ClassType>(b => { });
    }

    [Fact]
    public void CanConfigureRecordClass()
    {
        Options.WithHigherPriority<RecordType>(b => { });
    }

    [Fact]
    public void CanConfigureStruct()
    {
        Options.WithHigherPriority<StructType>(b => { });
    }

    [Fact]
    public void CanConfigureRecordStruct()
    {
        Options.WithHigherPriority<RecordStructType>(b => { });
    }

    [Fact]
    public void CanConfigureReadonlyRecordStruct()
    {
        Options.WithHigherPriority<ReadonlyRecordStructType>(b => { });
    }

    [Fact]
    public void ThrowsOnPrimitiveTypes()
    {
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<byte>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<sbyte>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<short>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<ushort>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<int>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<uint>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<long>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<ulong>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<float>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<double>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<decimal>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<char>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<bool>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<string>(b => { }));
    }

    [Fact]
    public void ThrowsOnEnumTypes()
    {
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<EnumType>(b => { }));
    }

    [Fact]
    public void ThrowsOnEnumerableTypes()
    {
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<ClassType[]>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<IEnumerable<ClassType>>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<List<ClassType>>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<Dictionary<string, ClassType>>(b => { }));
    }

    [Fact]
    public void ThrowsOnDateTimeTypes()
    {
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<DateTime>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<DateTimeOffset>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<DateOnly>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<TimeOnly>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.WithHigherPriority<TimeSpan>(b => { }));
    }

    public class ClassType { }
    public record RecordType { }
    public struct StructType { }
    public record struct RecordStructType { }
    public readonly record struct ReadonlyRecordStructType { }
    public enum EnumType { }
}
