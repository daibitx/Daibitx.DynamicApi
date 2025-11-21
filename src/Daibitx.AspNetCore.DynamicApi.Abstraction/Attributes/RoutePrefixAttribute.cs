using System;

namespace Daibitx.AspNetCore.DynamicApi.Abstraction.Attributes;
/// <summary>
/// represents the route prefix for a dynamic API controller.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public sealed class RoutePrefixAttribute : Attribute
{
    public string Prefix { get; }

    public RoutePrefixAttribute(string prefix)
    {
        Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
    }
}
