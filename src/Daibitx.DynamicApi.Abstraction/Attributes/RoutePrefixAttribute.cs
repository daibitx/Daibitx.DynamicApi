using System;

namespace Daibitx.DynamicApi.Abstraction.Attributes;
/// <summary>
/// 路由前缀特性
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
