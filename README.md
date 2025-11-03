# Sparcpoint.Extensions.Json

A **Fluent API** for customizing JSON serialization in **.NET** using `System.Text.Json`.

This library provides a fluent, code-based alternative to using attributes (like `[JsonPropertyName]`, `[JsonIgnore]`, etc.).
It allows you to configure how types are serialized **externally**—without modifying the source code of those types.

Ideal for:

* Sharing the same model across multiple serialization contexts.
* Avoiding attribute clutter.
* Dynamically applying serialization rules at runtime.

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package Sparcpoint.Extensions.Json
```

---

## 🚀 Getting Started

You configure serialization rules using the fluent `JsonEntityBuilder<T>` API.
These configurations can be applied to `JsonSerializerOptions` with **higher** or **lower** priority.

```csharp
using System.Text.Json;

var options = new JsonSerializerOptions()
    .WithHigherPriority<Person>(entity => entity
        .Property(p => p.FirstName).Name("first_name")
        .Property(p => p.LastName).Ignore()
        .Property(p => p.Age).Required()
    );

var json = JsonSerializer.Serialize(new Person
{
    FirstName = "John",
    LastName = "Doe",
    Age = 42
}, options);

Console.WriteLine(json);
// Output: {"first_name":"John","Age":42}
```

---

## 🧱 Fluent API Overview

### `JsonEntityBuilder<TEntity>`

Represents configuration for a single entity type.
Used internally by the `.WithHigherPriority<T>()` and `.WithLowerPriority<T>()` extension methods.

You typically won’t instantiate this directly—instead, you pass a lambda to configure it:

```csharp
options.WithLowerPriority<User>(entity => {
    entity.Property(u => u.Email).Ignore();
});
```

---

### `Property()`

Selects a property on the entity to configure.

```csharp
entity.Property(u => u.Email)
```

This returns a `JsonMemberBuilder<TEntity, TProperty>` for chaining member-level configuration.

---

### Member Configuration Methods

| Method                                                          | Description                                        |
| --------------------------------------------------------------- | -------------------------------------------------- |
| `.Name(string name)`                                            | Changes the JSON property name.                    |
| `.Ignore()`                                                     | Excludes the property from serialization.          |
| `.Order(int order)`                                             | Sets the order of the property in serialized JSON. |
| `.Required()`                                                   | Marks the property as required.                    |
| `.NumberHandling(JsonNumberHandling? handling)`                 | Applies a custom number handling rule.             |
| `.ObjectCreationHandling(JsonObjectCreationHandling? handling)` | Controls object creation behavior.                 |
| `.CustomConverter(JsonConverter converter)`                     | Assigns a custom converter for this property.      |

Example:

```csharp
entity.Property(u => u.Id)
      .Order(1)
      .Required()
      .Name("user_id");
```

---

## ⚙️ Applying Configuration to JsonSerializerOptions

Two extension methods are provided for integrating your configurations:

### `.WithHigherPriority<T>()`

Adds the configuration at the *front* of the `TypeInfoResolverChain`.
This means it takes precedence over existing resolvers.

```csharp
options.WithHigherPriority<MyModel>(builder => {
    builder.Property(m => m.Secret).Ignore();
});
```

### `.WithLowerPriority<T>()`

Adds the configuration at the *end* of the `TypeInfoResolverChain`.
This means existing resolvers take precedence over it.

```csharp
options.WithLowerPriority<MyModel>(builder => {
    builder.Property(m => m.Description).Name("desc");
});
```

Both methods support an optional parameter to control inheritance:

```csharp
.WithHigherPriority<MyModel>(builder => { ... }, includeInheritedTypes: true)
```

---

## 🧩 Example: Combining Multiple Rules

You can chain multiple configuration calls for clarity:

```csharp
options
    .WithHigherPriority<Order>(entity => entity
        .Property(o => o.OrderId).Name("id")
        .Property(o => o.CustomerName).Name("customer")
        .Property(o => o.InternalNotes).Ignore()
    )
    .WithLowerPriority<Customer>(entity => entity
        .Property(c => c.Email).Required()
    );
```

---

## 🧠 Why Fluent Configuration?

This API helps you:

* Keep DTOs free of serialization attributes.
* Apply serialization behavior conditionally (e.g., per environment or feature flag).
* Centralize serialization configuration in one place.
* Modify serialization for third-party or auto-generated classes without source changes.

---

## 🧩 Notes and Constraints

* Only **classes** and **structs** can be configured (no primitives, enums, arrays, or collections).
* System types (in `System.*` namespaces) are excluded by default.
* Configurations are applied via `IJsonTypeInfoResolver` modifiers.

---

## 🧪 Example: Dynamic Behavior

```csharp
if (Environment.GetEnvironmentVariable("APP_ENV") == "Production")
{
    options.WithHigherPriority<User>(b =>
        b.Property(u => u.Password).Ignore()
    );
}
```

---

## 🧾 License

MIT License © Sparcpoint
