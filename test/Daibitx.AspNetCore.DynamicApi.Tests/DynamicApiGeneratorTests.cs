using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daibitx.AspNetCore.DynamicApi.Abstraction.Attributes;
using Daibitx.AspNetCore.DynamicApi.Abstraction.Interfaces;
using Daibitx.AspNetCore.DynamicApi.Runtime.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Daibitx.AspNetCore.DynamicApi.Tests;

/// <summary>
/// DynamicApiGenerator 集成测试
/// </summary>
public class DynamicApiGeneratorTests
{
    private const string TestInterfaceCode = @"
using Daibitx.AspNetCore.DynamicApi.Abstraction.Attributes;
using Daibitx.AspNetCore.DynamicApi.Abstraction.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestNamespace
{
   [RoutePrefix(""/api/user/test"")]
    public interface IUserService : IDynamicController
    {
        Task<UserDto> GetUserAsync(long id);
        Task<List<UserDto>> GetUsersAsync(string name);
        Task CreateUserAsync(CreateUserDto dto);
        Task UpdateUserAsync(long id, UpdateUserDto dto);
        Task DeleteUserAsync(long id);
    }

    public class UserDto { public long Id { get; set; } public string Name { get; set; } }
    public class CreateUserDto { public string Name { get; set; } }
    public class UpdateUserDto { public string Name { get; set; } }
}";

    [Fact]
    public void GenerateController_ShouldCreateValidController()
    {
        // Arrange
        var syntaxTree = CSharpSyntaxTree.ParseText(TestInterfaceCode);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IDynamicController).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(RoutePrefixAttribute).Assembly.Location)
        };
        
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Act
        var generator = new DynamicApiGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        var generatedTrees = outputCompilation.SyntaxTrees.Where(t => t.FilePath.Contains(".g.cs")).ToList();
        Assert.NotEmpty(generatedTrees);
        
        var generatedCode = generatedTrees.First().ToString();
        Assert.Contains("public partial class UserServiceController", generatedCode);
        Assert.Contains("[Route(\"api/v1/users\")]", generatedCode);
        Assert.Contains("private readonly IUserService _service;", generatedCode);
    }

    [Fact]
    public void GenerateController_ShouldGenerateCorrectHttpMethods()
    {
        // Arrange
        var syntaxTree = CSharpSyntaxTree.ParseText(TestInterfaceCode);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IDynamicController).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(RoutePrefixAttribute).Assembly.Location)
        };
        
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Act
        var generator = new DynamicApiGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        var generatedTrees = outputCompilation.SyntaxTrees.Where(t => t.FilePath.Contains(".g.cs")).ToList();
        var generatedCode = generatedTrees.First().ToString();
        
        Assert.Contains("[HttpGet]", generatedCode);
        Assert.Contains("[HttpPost]", generatedCode);
        Assert.Contains("[HttpPut]", generatedCode);
        Assert.Contains("[HttpDelete]", generatedCode);
    }

    [Fact]
    public void GenerateController_ShouldGenerateCorrectParameters()
    {
        // Arrange
        var syntaxTree = CSharpSyntaxTree.ParseText(TestInterfaceCode);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IDynamicController).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(RoutePrefixAttribute).Assembly.Location)
        };
        
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Act
        var generator = new DynamicApiGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        var generatedTrees = outputCompilation.SyntaxTrees.Where(t => t.FilePath.Contains(".g.cs")).ToList();
        var generatedCode = generatedTrees.First().ToString();
        
        // 验证参数绑定
        Assert.Contains("[FromRoute] long id", generatedCode);
        Assert.Contains("[FromQuery] string name", generatedCode);
        Assert.Contains("[FromBody] CreateUserDto dto", generatedCode);
        Assert.Contains("[FromBody] UpdateUserDto dto", generatedCode);
    }

    [Fact]
    public void GenerateController_ShouldHandleInterfacesWithoutRoutePrefix()
    {
        // Arrange
        var codeWithoutPrefix = @"
using Daibitx.AspNetCore.DynamicApi.Abstraction.Interfaces;
using System.Threading.Tasks;

namespace TestNamespace
{
    public interface IProductService : IDynamicController
    {
        Task GetProductAsync(long id);
    }
}";
        
        var syntaxTree = CSharpSyntaxTree.ParseText(codeWithoutPrefix);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IDynamicController).Assembly.Location)
        };
        
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Act
        var generator = new DynamicApiGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        var generatedTrees = outputCompilation.SyntaxTrees.Where(t => t.FilePath.Contains(".g.cs")).ToList();
        Assert.NotEmpty(generatedTrees);
        
        var generatedCode = generatedTrees.First().ToString();
        Assert.Contains("[Route(\"/api/iproductservice\")]", generatedCode);
    }

    [Fact]
    public void GenerateController_ShouldNotGenerate_ForNonDynamicControllerInterfaces()
    {
        // Arrange
        var code = @"
namespace TestNamespace
{
    public interface IRegularService
    {
        void DoSomething();
    }
}";
        
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
        
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Act
        var generator = new DynamicApiGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        var generatedTrees = outputCompilation.SyntaxTrees.Where(t => t.FilePath.Contains(".g.cs")).ToList();
        Assert.Empty(generatedTrees);
    }
}