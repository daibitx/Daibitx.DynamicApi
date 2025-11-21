using System;

using Microsoft.CodeAnalysis;

namespace Daibitx.AspNetCore.DynamicApi.Runtime.Generators
{
    /// <summary>
    /// 参数绑定解析器 - 自动推导参数的绑定源
    /// </summary>
    public static class ParameterBindingResolver
    {
        /// <summary>
        /// 解析参数的绑定源
        /// </summary>
        public static string Resolve(IParameterSymbol param, string httpMethod)
        {
            var type = param.Type as INamedTypeSymbol;

            // 规则 1: IFormFile → FromForm
            if (IsFormFile(type))
            {
                return "[FromForm]";
            }

            // 规则 2: 复杂类型（DTO）→ FromBody
            if (!IsSimpleType(type))
            {
                return "[FromBody]";
            }

            // 规则 3: 参数名含 id/key/code → FromRoute
            if (IsRouteParameter(param.Name))
            {
                return "[FromRoute]";
            }

            // 规则 4: GET/DELETE → FromQuery
            if (httpMethod is "HttpGet" or "HttpDelete")
            {
                return "[FromQuery]";
            }

            // 规则 5: 默认 FromQuery
            return "[FromQuery]";
        }

        /// <summary>
        /// 判断是否为简单类型（值类型、string、decimal等）
        /// </summary>
        private static bool IsSimpleType(INamedTypeSymbol type)
        {
            if (type == null) return false;

            // 值类型
            if (type.IsValueType) return true;

            // string
            if (type.SpecialType == SpecialType.System_String) return true;

            // 其他特殊类型
            switch (type.SpecialType)
            {
                case SpecialType.System_Object:
                case SpecialType.System_Decimal:
                case SpecialType.System_DateTime:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 判断是否为表单文件类型
        /// </summary>
        private static bool IsFormFile(INamedTypeSymbol type)
        {
            if (type == null) return false;

            return type.Name == "IFormFile" ||
                   type.Name == "IFormFileCollection" ||
                   type.Name == "List" && type.TypeArguments.Length > 0 &&
                    type.TypeArguments[0].Name == "IFormFile";
        }

        /// <summary>
        /// 判断是否为路由参数
        /// </summary>
        private static bool IsRouteParameter(string paramName)
        {
            if (string.IsNullOrEmpty(paramName)) return false;

            var lowerName = paramName.ToLowerInvariant();
            return lowerName.Contains("id") ||
                   lowerName.Contains("key") ||
                   lowerName.Contains("code");
        }

        /// <summary>
        /// 获取参数的默认值字符串表示
        /// </summary>
        public static string GetDefaultValue(IParameterSymbol param)
        {
            if (!param.IsOptional || param.HasExplicitDefaultValue == false)
            {
                return "default";
            }

            if (param.ExplicitDefaultValue == null)
            {
                return "null";
            }

            if (param.Type.SpecialType == SpecialType.System_Boolean)
            {
                return ((bool)param.ExplicitDefaultValue).ToString().ToLower();
            }

            return param.ExplicitDefaultValue.ToString();
        }
    }
}