using Daibitx.AspNetCore.DynamicApi.Runtime.Models;
using Daibitx.AspNetCore.DynamicApi.Runtime.Templates;
using Xunit;

namespace Daibitx.AspNetCore.DynamicApi.Tests;

/// <summary>
/// ControllerTemplate UnitTest
/// </summary>
public class ControllerTemplateTests
{
    private readonly ControllerTemplate _template;

    public ControllerTemplateTests()
    {
        _template = new ControllerTemplate();
    }

    [Fact]
    public void Generate_ShouldCreateValidController()
    {
        // Arrange
        var methods = new List<MethodInfo>
        {
            new MethodInfo
            {
                Name = "GetUser",
                ReturnType = "global::System.Threading.Tasks.Task<global::TestNamespace.UserDto>",
                HttpMethod = "HttpGet",
                Parameters = new List<ParameterInfo>
                {
                    new ParameterInfo
                    {
                        Name = "id",
                        Type = "global::System.Int64",
                        BindingSource = "[FromRoute]",
                        IsOptional = false
                    }
                }
            }
        };

        var apiExplorerSettings = new ApiExplorerSettings
        {
            GroupName = "Users",
            IgnoreApi = false
        };

        // Act
        var result = _template.Generate(
            "TestNamespace",
            "UserServiceController",
            "IUserService",
            "api/v1/users",
            apiExplorerSettings,
            methods);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("namespace TestNamespace", result);
        Assert.Contains("public partial class UserServiceController", result);
        Assert.Contains("private readonly IUserService _service;", result);
        Assert.Contains("[Route(\"api/v1/users\")]", result);
        Assert.Contains("[ApiExplorerSettings(GroupName = \"Users\")]", result);
        Assert.Contains("[HttpGet]", result);
        Assert.Contains("public async", result);
        Assert.Contains("GetUser(", result);
        Assert.Contains("global::System.Threading.Tasks.Task<global::TestNamespace.UserDto>", result);
        Assert.Contains("[FromRoute] global::System.Int64 id", result);
    }

    [Fact]
    public void Generate_ShouldHandleEmptyMethods()
    {
        // Arrange
        var methods = new List<MethodInfo>();
        var apiExplorerSettings = new ApiExplorerSettings();

        // Act
        var result = _template.Generate(
            "TestNamespace",
            "EmptyController",
            "IEmptyService",
            "api/empty",
            apiExplorerSettings,
            methods);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("public partial class EmptyController", result);
        Assert.Contains("private readonly IEmptyService _service;", result);
    }

    [Fact]
    public void Generate_ShouldHandleOptionalParameters()
    {
        // Arrange
        var methods = new List<MethodInfo>
        {
            new MethodInfo
            {
                Name = "SearchUsers",
                ReturnType = "global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<global::TestNamespace.UserDto>>",
                HttpMethod = "HttpGet",
                Parameters = new List<ParameterInfo>
                {
                    new ParameterInfo
                    {
                        Name = "name",
                        Type = "global::System.String",
                        BindingSource = "[FromQuery]",
                        IsOptional = true,
                        DefaultValue = "null"
                    },
                    new ParameterInfo
                    {
                        Name = "page",
                        Type = "global::System.Int32",
                        BindingSource = "[FromQuery]",
                        IsOptional = true,
                        DefaultValue = "1"
                    }
                }
            }
        };

        var apiExplorerSettings = new ApiExplorerSettings();

        // Act
        var result = _template.Generate(
            "TestNamespace",
            "UserServiceController",
            "IUserService",
            "api/users",
            apiExplorerSettings,
            methods);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("[FromQuery] global::System.String name = null", result);
        Assert.Contains("[FromQuery] global::System.Int32 page = 1", result);
    }

    [Fact]
    public void Generate_ShouldHandleIgnoreApi()
    {
        // Arrange
        var methods = new List<MethodInfo>();
        var apiExplorerSettings = new ApiExplorerSettings
        {
            IgnoreApi = true
        };

        // Act
        var result = _template.Generate(
            "TestNamespace",
            "HiddenController",
            "IHiddenService",
            "api/hidden",
            apiExplorerSettings,
            methods);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("[ApiExplorerSettings(IgnoreApi = true)]", result);
    }

    [Fact]
    public void Generate_ShouldGenerateRouteTemplate_ForGetMethodsWithId()
    {
        // Arrange
        var methods = new List<MethodInfo>
        {
            new MethodInfo
            {
                Name = "GetUser",
                ReturnType = "global::System.Threading.Tasks.Task<global::TestNamespace.UserDto>",
                HttpMethod = "HttpGet",
                Parameters = new List<ParameterInfo>
                {
                    new ParameterInfo
                    {
                        Name = "id",
                        Type = "global::System.Int64",
                        BindingSource = "[FromRoute]",
                        IsOptional = false
                    }
                }
            }
        };

        var apiExplorerSettings = new ApiExplorerSettings();

        // Act
        var result = _template.Generate(
            "TestNamespace",
            "UserServiceController",
            "IUserService",
            "api/users",
            apiExplorerSettings,
            methods);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("[Route(\"{id}\")]", result);
    }

    [Fact]
    public void Generate_ShouldNotGenerateRouteTemplate_ForPostMethods()
    {
        // Arrange
        var methods = new List<MethodInfo>
        {
            new MethodInfo
            {
                Name = "CreateUser",
                ReturnType = "global::System.Threading.Tasks.Task",
                HttpMethod = "HttpPost",
                Parameters = new List<ParameterInfo>
                {
                    new ParameterInfo
                    {
                        Name = "dto",
                        Type = "global::TestNamespace.CreateUserDto",
                        BindingSource = "[FromBody]",
                        IsOptional = false
                    }
                }
            }
        };

        var apiExplorerSettings = new ApiExplorerSettings();

        // Act
        var result = _template.Generate(
            "TestNamespace",
            "UserServiceController",
            "IUserService",
            "api/users",
            apiExplorerSettings,
            methods);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("[Route(\"{id}\")]", result);
    }

    [Fact]
    public void Generate_ShouldGenerateCorrectConstructor()
    {
        // Arrange
        var methods = new List<MethodInfo>();
        var apiExplorerSettings = new ApiExplorerSettings();

        // Act
        var result = _template.Generate(
            "TestNamespace",
            "TestController",
            "ITestService",
            "api/test",
            apiExplorerSettings,
            methods);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("public TestController(ITestService service)", result);
        Assert.Contains("_service = service ?? throw new ArgumentNullException(nameof(service));", result);
    }

    [Fact]
    public void Generate_ShouldGenerateMethodCalls()
    {
        // Arrange
        var methods = new List<MethodInfo>
        {
            new MethodInfo
            {
                Name = "GetUser",
                ReturnType = "global::System.Threading.Tasks.Task<global::TestNamespace.UserDto>",
                HttpMethod = "HttpGet",
                Parameters = new List<ParameterInfo>
                {
                    new ParameterInfo
                    {
                        Name = "id",
                        Type = "global::System.Int64",
                        BindingSource = "[FromRoute]",
                        IsOptional = false
                    }
                }
            }
        };

        var apiExplorerSettings = new ApiExplorerSettings();

        // Act
        var result = _template.Generate(
            "TestNamespace",
            "UserServiceController",
            "IUserService",
            "api/users",
            apiExplorerSettings,
            methods);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("return await _service.GetUser(id);", result);
    }
}