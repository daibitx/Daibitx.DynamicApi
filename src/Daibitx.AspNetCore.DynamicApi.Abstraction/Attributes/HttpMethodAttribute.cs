using System;

namespace Daibitx.AspNetCore.DynamicApi.Abstraction.Attributes;
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class HttpMethodAttribute : Attribute
{
    public DynamicMethod Method { get; }

    public HttpMethodAttribute(DynamicMethod method = DynamicMethod.Post)
    {
        Method = method;
    }

}

public enum DynamicMethod
{
    Get,
    Post,
    Put,
    Delete,
    Patch,
    Head,
    Options,
    Trace
}
