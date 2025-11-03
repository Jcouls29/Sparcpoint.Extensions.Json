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
        Options.Configure<ClassType>(b => { });
    }

    [Fact]
    public void CanConfigureRecordClass()
    {
        Options.Configure<RecordType>(b => { });
    }

    [Fact]
    public void CanConfigureStruct()
    {
        Options.Configure<StructType>(b => { });
    }

    [Fact]
    public void CanConfigureRecordStruct()
    {
        Options.Configure<RecordStructType>(b => { });
    }

    [Fact]
    public void CanConfigureReadonlyRecordStruct()
    {
        Options.Configure<ReadonlyRecordStructType>(b => { });
    }

    [Fact]
    public void ThrowsOnPrimitiveTypes()
    {
        Assert.Throws<InvalidOperationException>(() => Options.Configure<byte>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<sbyte>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<short>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<ushort>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<int>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<uint>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<long>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<ulong>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<float>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<double>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<decimal>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<char>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<bool>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<string>(b => { }));
    }

    [Fact]
    public void ThrowsOnEnumTypes()
    {
        Assert.Throws<InvalidOperationException>(() => Options.Configure<EnumType>(b => { }));
    }

    [Fact]
    public void ThrowsOnEnumerableTypes()
    {
        Assert.Throws<InvalidOperationException>(() => Options.Configure<ClassType[]>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<IEnumerable<ClassType>>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<List<ClassType>>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<Dictionary<string, ClassType>>(b => { }));
    }

    [Fact]
    public void ThrowsOnDateTimeTypes()
    {
        Assert.Throws<InvalidOperationException>(() => Options.Configure<DateTime>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<DateTimeOffset>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<DateOnly>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<TimeOnly>(b => { }));
        Assert.Throws<InvalidOperationException>(() => Options.Configure<TimeSpan>(b => { }));
    }

    public class ClassType { }
    public record RecordType { }
    public struct StructType { }
    public record struct RecordStructType { }
    public readonly record struct ReadonlyRecordStructType { }
    public enum EnumType { }
}
