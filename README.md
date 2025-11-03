# Sparcpoint.Extensions.Json

Extensions for System.Text.Json

# Installation

You should install with NuGet:

```powershell
Install-Package Sparcpoint.Extensions.Json
```

# Fluent API

The Fluent API provides a flexible, attribute-free approach to JSON serialization. Instead of relying on annotations or modifying your data models directly, you can define how objects are serialized externally through fluent configuration.

This approach is especially useful when the same data object needs to be serialized differently across multiple use cases—for example, sharing only certain fields in one context while including more details in another. By doing so, it reduces the need for redundant DTOs or specialized view models, minimizing complexity and improving maintainability.

In short, the Fluent API gives you complete control over how data is represented in JSON—without requiring direct access or modifications to the underlying objects.

## Quick Start

The core of the fluent API is built around tailoring the `JsonSerializerOptions` for each type.

**Basic Use Case**

Create a new `JsonSerializerOptions` object and start customizing

```csharp
var options = new JsonSerializerOptions();

options.Configure<ExampleType>(b => b
  // Changes the name of the property in JSON to `prop-name`
  .Property(p => p.MyPropertyName).Name("prop-name")
  // Ignores this property during serialization
  .Property(p => p.IgnoredProperty).Ignore()
  // Adds a new property called `fullname` with the value provided
  .ComputedProperty("fullname", p => p.FirstName + " " + p.LastName)
  // Uses a custom converter for the given property
  .Property(p => p.UniqueProperty).Converter(new CustomConverter())
);
```

**Encapsulate Logic into Classes**

```csharp
public static class ExampleTypeSerializerModel
{
  public static JsonModelBuilder Setup<ExampleType>(JsonModelBuilder<ExampleType> builder)
  {
    builder.Property(p => p.MyPropertyName).Name("prop-name");
    // Ignores this property during serialization
    builder.Property(p => p.IgnoredProperty).Ignore();
    // Adds a new property called `fullname` with the value provided
    builder.ComputedProperty("fullname", p => p.FirstName + " " + p.LastName);
    // Uses a custom converter for the given property
    builder.Property(p => p.UniqueProperty).Converter(new CustomConverter());

    return bulider;
  }
}

var options = new JsonSerializerOptions();
options.Configure(ExampleTypeSerializerModel.Setup);
```
