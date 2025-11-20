using System;

namespace Daibitx.DynamicApi.Attributes;
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class HttpMethodAttribute : Attribute
{
    public HttpMethod Method { get; }

    public HttpMethodAttribute(HttpMethod method = HttpMethod.Post)
    {
        Method = method;
    }

}

public enum HttpMethod
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
