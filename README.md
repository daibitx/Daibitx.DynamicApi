English | [简体中文](README.zh_CN.md)

## Project Overview

Automatically generates Web API controllers from interface definitions.

## Key Features

- 🚀 **Compile-time generation** – Uses Roslyn source generators to generate controller code during compilation
- 🔒 **Type-safe** – Fully based on interface definitions with compile-time type checking
- 🎯 **Smart routing** – Automatically generates routes based on method names and parameters
- 📦 **Parameter binding** – Intelligently detects parameter sources (path, query, body)
- 📚 **API documentation support** – Fully compatible with Swagger/OpenAPI
- ⚡ **High performance** – No runtime reflection overhead
- 🔧 **Flexible configuration** – Supports custom route prefixes and API documentation metadata

## Project Structure

```
src/
├── Daibitx.AspNetCore.DynamicApi.sln          # Solution file
├── Daibitx.AspNetCore.DynamicApi/             # Main package
├── Daibitx.AspNetCore.DynamicApi.Abstraction/ # Abstractions (interfaces & attributes)
└── Daibitx.AspNetCore.DynamicApi.Runtime/     # Source generator implementation
demo/
└── WebApplication1/                           # Demo application
test/
└── Daibitx.AspNetCore.DynamicApi.Tests/       # Unit tests
```

## Installation

Install via NuGet:

```
dotnet add package Daibitx.AspNetCore.DynamicApi
```

------

## Quick Start

### 1. Define a Service Interface

```
using Daibitx.AspNetCore.DynamicApi.Abstraction.Attributes;
using Daibitx.AspNetCore.DynamicApi.Abstraction.Interfaces;

namespace DemoApp.Services;

[RoutePrefix("/api/sample")]
[ApiExplorerSettings(false, "Sample Service")]
public interface ISampleService : IDynamicController
{
    // GET example, no parameters
    Task<string> GetWelcomeMessage();

    // GET example with route and query parameters
    [HttpMethod(DynamicMethod.Get)]
    Task<string> GetItemAsync(int id, string keyword);

    // POST example with body parameter
    [HttpMethod(DynamicMethod.Post)]
    Task<bool> CreateItemAsync(SampleCreateModel model);

    // PUT example with route parameter + body
    [HttpMethod(DynamicMethod.Put)]
    Task<bool> UpdateItemAsync(int id, SampleUpdateModel model);

    // DELETE example with optional parameter
    [HttpMethod(DynamicMethod.Delete)]
    Task<bool> DeleteItemAsync(int id, bool force = false);

    // Example of list retrieval
    [HttpMethod(DynamicMethod.Get)]
    Task<IEnumerable<SampleListItem>> GetListAsync(int pageIndex = 1, int pageSize = 10);
}
```

### 2. Implement the Interface

```
public class SampleService : ISampleService
{
    public Task<string> GetWelcomeMessage()
        => Task.FromResult("Welcome to Dynamic API!");

    public Task<string> GetItemAsync(int id, string keyword)
        => Task.FromResult($"Item {id} with keyword: {keyword}");

    public Task<bool> CreateItemAsync(SampleCreateModel model)
        => Task.FromResult(true);

    // Other method implementations...
}
```

### 3. Register the Service

In `Program.cs`:

```
var builder = WebApplication.CreateBuilder(args);

// Register service
builder.Services.AddScoped<ISampleService, SampleService>();

var app = builder.Build();
app.Run();
```

### 4. Automatically Generated APIs

After compilation, the following endpoints will be generated:

- `GET /api/sample/getwelcomemessage` – Get welcome message
- `GET /api/sample/getitemasync/{id}` – Get specific item
- `POST /api/sample/createitemasync` – Create an item
- `PUT /api/sample/updateitemasync/{id}` – Update an item
- `DELETE /api/sample/deleteitemasync/{id}` – Delete an item
- `GET /api/sample/getlistasync` – Fetch a list

------

## Advanced Features

### Custom Route Configuration

```
[RoutePrefix("/api/v1/users")]
public interface IUserService : IDynamicController
{
    // ...
}
```

### API Documentation Metadata

```
[ApiExplorerSettings(false, "User Management")]
public interface IUserService : IDynamicController
{
    // ...
}
```

### HTTP Method Specification

```
public interface IOrderService : IDynamicController
{
    [HttpMethod(DynamicMethod.Post)]
    Task<bool> CreateOrderAsync(OrderModel order);

    [HttpMethod(DynamicMethod.Put)]
    Task<bool> UpdateOrderAsync(int orderId, OrderModel order);

    [HttpMethod(DynamicMethod.Delete)]
    Task<bool> CancelOrderAsync(int orderId);
}
```

### Parameter Binding Rules

The system automatically determines parameter binding sources:

- **Path parameters** – First parameter (commonly an ID)
- **Query parameters** – Simple types (`string`, `int`, `bool`, etc.)
- **Body parameters** – Complex object types
- **Optional parameters** – Supported with default values

------

## Technical Architecture

### Core Components

1. **Daibitx.AspNetCore.DynamicApi.Abstraction**
   - Defines the core interface `IDynamicController`
   - Provides configuration attributes (`RoutePrefixAttribute`, `HttpMethodAttribute`, etc.)
   - Targets **.NET Standard 2.0**
2. **Daibitx.AspNetCore.DynamicApi.Runtime**
   - Roslyn source generator implementation
   - Analyzes interface definitions during compilation
   - Generates ASP.NET Core controller code
   - Targets **.NET Standard 2.0**
3. **Daibitx.AspNetCore.DynamicApi**
   - Main package bundling the above components
   - Provides full dynamic API generation capability

### Generation Workflow

1. **Syntax Analysis** – Roslyn inspects interfaces implementing `IDynamicController`
2. **Semantic Analysis** – Extracts methods, parameters, and attribute metadata
3. **Code Generation** – Templates generate controller classes
4. **Compilation Integration** – Generated code becomes part of the final build

