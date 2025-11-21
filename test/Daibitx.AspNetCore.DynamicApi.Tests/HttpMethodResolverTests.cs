using Daibitx.AspNetCore.DynamicApi.Runtime.Generators;
using Xunit;

namespace Daibitx.AspNetCore.DynamicApi.Tests;

/// <summary>
/// HttpMethodResolver 单元测试
/// </summary>
public class HttpMethodResolverTests
{
    [Theory]
    [InlineData("GetUser", "HttpGet")]
    [InlineData("GetAllUsers", "HttpGet")]
    [InlineData("FindUser", "HttpGet")]
    [InlineData("QueryUsers", "HttpGet")]
    [InlineData("SearchUsers", "HttpGet")]
    [InlineData("FetchData", "HttpGet")]
    [InlineData("RetrieveInfo", "HttpGet")]
    public void Resolve_ShouldReturnHttpGet_ForGetPrefixes(string methodName, string expected)
    {
        // Act
        var result = HttpMethodResolver.Resolve(methodName);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("CreateUser", "HttpPost")]
    [InlineData("AddUser", "HttpPost")]
    [InlineData("InsertRecord", "HttpPost")]
    [InlineData("PostData", "HttpPost")]
    [InlineData("SubmitForm", "HttpPost")]
    public void Resolve_ShouldReturnHttpPost_ForPostPrefixes(string methodName, string expected)
    {
        // Act
        var result = HttpMethodResolver.Resolve(methodName);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("UpdateUser", "HttpPut")]
    [InlineData("EditUser", "HttpPut")]
    [InlineData("ModifyData", "HttpPut")]
    [InlineData("PutData", "HttpPut")]
    [InlineData("ReplaceItem", "HttpPut")]
    public void Resolve_ShouldReturnHttpPut_ForPutPrefixes(string methodName, string expected)
    {
        // Act
        var result = HttpMethodResolver.Resolve(methodName);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("DeleteUser", "HttpDelete")]
    [InlineData("RemoveUser", "HttpDelete")]
    [InlineData("DestroyData", "HttpDelete")]
    [InlineData("DropTable", "HttpDelete")]
    public void Resolve_ShouldReturnHttpDelete_ForDeletePrefixes(string methodName, string expected)
    {
        // Act
        var result = HttpMethodResolver.Resolve(methodName);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("PatchUser", "HttpPatch")]
    [InlineData("PartialUpdate", "HttpPatch")]
    public void Resolve_ShouldReturnHttpPatch_ForPatchPrefixes(string methodName, string expected)
    {
        // Act
        var result = HttpMethodResolver.Resolve(methodName);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("ProcessData", "HttpPost")]
    [InlineData("Calculate", "HttpPost")]
    [InlineData("Execute", "HttpPost")]
    public void Resolve_ShouldReturnHttpPost_ForUnknownPrefixes(string methodName, string expected)
    {
        // Act
        var result = HttpMethodResolver.Resolve(methodName);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Resolve_ShouldReturnHttpPost_ForEmptyMethodName()
    {
        // Act
        var result = HttpMethodResolver.Resolve("");
        
        // Assert
        Assert.Equal("HttpPost", result);
    }

    [Fact]
    public void Resolve_ShouldReturnHttpPost_ForNullMethodName()
    {
        // Act
        var result = HttpMethodResolver.Resolve(null);
        
        // Assert
        Assert.Equal("HttpPost", result);
    }
}