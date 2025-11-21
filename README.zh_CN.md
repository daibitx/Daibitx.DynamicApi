简体中文| [English](README.md)

## 简介

一个基于 Roslyn 源代码生成器的 ASP.NET Core 动态 API 生成库，通过接口定义自动生成 Web API 控制器

## 核心特性

- 🚀 **编译时生成** - 使用 Roslyn 源代码生成器，在编译时生成控制器代码
- 🔒 **类型安全** - 完全基于接口定义，提供编译时类型检查
- 🎯 **智能路由** - 自动根据方法名和参数生成路由
- 📦 **参数绑定** - 智能识别参数来源（路径、查询、请求体）
- 📚 **API 文档支持** - 支持 Swagger/OpenAPI 集成
- ⚡ **高性能** - 零运行时反射开销
- 🔧 **灵活配置** - 支持自定义路由前缀和 API 文档设置

## 项目结构

```txt
src/
├── Daibitx.AspNetCore.DynamicApi.sln          # 解决方案文件
├── Daibitx.AspNetCore.DynamicApi/             # 主包
├── Daibitx.AspNetCore.DynamicApi.Abstraction/ # 抽象层（接口和特性）
└── Daibitx.AspNetCore.DynamicApi.Runtime/     # 源代码生成器
demo/
└── WebApplication1/                           # 示例应用
test/
└── Daibitx.AspNetCore.DynamicApi.Tests/       # 单元测试
```



## 安装

通过 NuGet 包管理器安装：

```bash
dotnet add package Daibitx.AspNetCore.DynamicApi
```



## 快速开始

### 1. 定义服务接口

```csharp
using Daibitx.AspNetCore.DynamicApi.Abstraction.Attributes;
using Daibitx.AspNetCore.DynamicApi.Abstraction.Interfaces;

namespace DemoApp.Services;

[RoutePrefix("/api/sample")]
[ApiExplorerSettings(false, "Sample Service")]
public interface ISampleService : IDynamicController
{
    // GET 示例，无参数
    Task<string> GetWelcomeMessage();

    // GET 示例，URL 参数 + Query 参数
    [HttpMethod(DynamicMethod.Get)]
    Task<string> GetItemAsync(int id, string keyword);

    // POST 示例，Body 参数
    [HttpMethod(DynamicMethod.Post)]
    Task<bool> CreateItemAsync(SampleCreateModel model);

    // PUT 示例，路径参数 + Body
    [HttpMethod(DynamicMethod.Put)]
    Task<bool> UpdateItemAsync(int id, SampleUpdateModel model);

    // DELETE 示例，带可选参数
    [HttpMethod(DynamicMethod.Delete)]
    Task<bool> DeleteItemAsync(int id, bool force = false);

    // 获取列表的示例
    [HttpMethod(DynamicMethod.Get)]
    Task<IEnumerable<SampleListItem>> GetListAsync(int pageIndex = 1, int pageSize = 10);
}
```



### 2. 实现接口

```csharp
public class SampleService : ISampleService
{
    public Task<string> GetWelcomeMessage()
    {
        return Task.FromResult("Welcome to Dynamic API!");
    }

    public Task<string> GetItemAsync(int id, string keyword)
    {
        return Task.FromResult($"Item {id} with keyword: {keyword}");
    }

    public Task<bool> CreateItemAsync(SampleCreateModel model)
    {
        // 实现创建逻辑
        return Task.FromResult(true);
    }

    // 其他方法实现...
}
```



### 3. 注册服务

在 `Program.cs` 中注册服务：

```csharp
var builder = WebApplication.CreateBuilder(args);

// 注册服务
builder.Services.AddScoped<ISampleService, SampleService>();

var app = builder.Build();
app.Run();
```



### 4. 自动生成的 API

编译后，将自动生成以下 API 端点：

- `GET /api/sample/getwelcomemessage` - 获取欢迎消息
- `GET /api/sample/getitemasync/{id}` - 获取指定项
- `POST /api/sample/createitemasync` - 创建新项
- `PUT /api/sample/updateitemasync/{id}` - 更新项
- `DELETE /api/sample/deleteitemasync/{id}` - 删除项
- `GET /api/sample/getlistasync` - 获取列表

## 高级特性

### 路由配置

使用 `[RoutePrefix]` 特性自定义路由前缀：

```csharp
[RoutePrefix("/api/v1/users")]
public interface IUserService : IDynamicController
{
    // ...
}
```

### API 文档设置

使用 `[ApiExplorerSettings]` 控制 API 文档生成：

```csharp
[ApiExplorerSettings(false, "User Management")]
public interface IUserService : IDynamicController
{
    // ...
}
```

### HTTP 方法指定

使用 `[HttpMethod]` 特性显式指定 HTTP 方法：

```csharp
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

### 参数绑定规则

系统自动识别参数绑定源：

- **路径参数**：第一个参数（通常是 ID）
- **查询参数**：简单类型参数（string, int, bool 等）
- **请求体**：复杂对象参数
- **可选参数**：支持默认值

## 技术架构

### 核心组件

1. **Daibitx.AspNetCore.DynamicApi.Abstraction**
   - 定义核心接口 `IDynamicController`
   - 提供配置特性（`RoutePrefixAttribute`, `HttpMethodAttribute`, `ApiExplorerSettingsAttribute`）
   - 目标框架：.NET Standard 2.0
2. **Daibitx.AspNetCore.DynamicApi.Runtime**
   - Roslyn 源代码生成器实现
   - 编译时分析接口定义
   - 生成 ASP.NET Core 控制器代码
   - 目标框架：.NET Standard 2.0
3. **Daibitx.AspNetCore.DynamicApi**
   - 主包，包含上述两个组件
   - 提供完整的动态 API 生成功能

### 生成流程

1. **语法分析**：Roslyn 分析器扫描实现 `IDynamicController` 的接口
2. **语义分析**：提取接口方法、参数、特性信息
3. **代码生成**：使用模板生成控制器类
4. **编译集成**：生成的代码作为编译的一部分
