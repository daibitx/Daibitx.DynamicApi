using System;

namespace Daibitx.AspNetCore.DynamicApi.Runtime.Generators
{
    /// <summary>
    /// HTTP 方法推导逻辑
    /// </summary>
    public static class HttpMethodResolver
    {
        private static readonly string[] GetPrefixes = { "Get", "Find", "Query", "Search", "Fetch", "Retrieve" };
        private static readonly string[] PostPrefixes = { "Create", "Add", "Insert", "Post", "Submit" };
        private static readonly string[] PutPrefixes = { "Update", "Edit", "Modify", "Put", "Replace" };
        private static readonly string[] DeletePrefixes = { "Delete", "Remove", "Destroy", "Drop" };
        private static readonly string[] PatchPrefixes = { "Patch", "PartialUpdate" };

        public static string Resolve(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                return "HttpPost"; //Default method
            }

            var upperMethod = methodName;

            // 检查各前缀
            if (MatchPrefix(upperMethod, GetPrefixes))
            {
                return "HttpGet";
            }

            if (MatchPrefix(upperMethod, PostPrefixes))
            {
                return "HttpPost";
            }

            if (MatchPrefix(upperMethod, PutPrefixes))
            {
                return "HttpPut";
            }

            if (MatchPrefix(upperMethod, DeletePrefixes))
            {
                return "HttpDelete";
            }

            if (MatchPrefix(upperMethod, PatchPrefixes))
            {
                return "HttpPatch";
            }
            //Default 
            return "HttpPost";
        }

        public static string ResolveFromEnumValue(string enumValue)
        {
            if (string.IsNullOrWhiteSpace(enumValue))
                return "HttpPost";
            return enumValue switch
            {
                "0" => "HttpGet",
                "1" => "HttpPost",
                "2" => "HttpPut",
                "3" => "HttpDelete",
                "4" => "HttpPatch",
                "5" => "HttpHead",
                "6" => "HttpOptions",
                "7" => "HttpTrace",
                _ => "HttpPost" //Defalut
            };
        }
        private static bool MatchPrefix(string methodName, string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                if (methodName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}