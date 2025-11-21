using System;

using Microsoft.CodeAnalysis;

namespace Daibitx.AspNetCore.DynamicApi.Runtime.Generators
{
    /// <summary>
    /// Parameter Binding Resolver - Automatically deduces the binding source of parameters
    /// </summary>
    public static class ParameterBindingResolver
    {
        /// <summary>
        /// Resolves the binding source of a parameters
        /// </summary>
        public static string Resolve(IParameterSymbol param, string httpMethod)
        {
            var type = param.Type as INamedTypeSymbol;

            if (IsFormFile(type))
            {
                return "[FromForm]";
            }

            if (!IsSimpleType(type))
            {
                return "[FromBody]";
            }

            if (IsRouteParameter(param.Name))
            {
                return "[FromRoute]";
            }

            if (httpMethod is "HttpGet" or "HttpDelete")
            {
                return "[FromQuery]";
            }

            return "[FromQuery]";
        }

        /// <summary>
        /// Judges whether the type is a simple type (value type, string, decimal, etc.)
        /// </summary>
        private static bool IsSimpleType(INamedTypeSymbol type)
        {
            if (type == null) return false;

            if (type.IsValueType) return true;

            if (type.SpecialType == SpecialType.System_String) return true;

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
        /// Judges whether the type is a form file type
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
        /// Judges whether the parameter is a route parameter
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
        /// Aquire the default value string representation of a parameter
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