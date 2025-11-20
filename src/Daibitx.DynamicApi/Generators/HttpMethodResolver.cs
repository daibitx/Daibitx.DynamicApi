using System;

namespace Daibitx.DynamicApi.Generators
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
                return "HttpPost"; // 默认方法
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

            // 默认根据参数决定
            return "HttpPost";
        }

        public static string Resolve(Daibitx.DynamicApi.Attributes.HttpMethod httpMethod)
        {
            switch (httpMethod)
            {
                case Daibitx.DynamicApi.Attributes.HttpMethod.Get:
                    return "HttpGet";
                case Daibitx.DynamicApi.Attributes.HttpMethod.Post:
                    return "HttpPost";
                case Daibitx.DynamicApi.Attributes.HttpMethod.Put:
                    return "HttpPut";
                case Daibitx.DynamicApi.Attributes.HttpMethod.Delete:
                    return "HttpDelete";
                case Daibitx.DynamicApi.Attributes.HttpMethod.Patch:
                    return "HttpPatch";
                case Daibitx.DynamicApi.Attributes.HttpMethod.Head:
                    return "HttpHead";
                case Daibitx.DynamicApi.Attributes.HttpMethod.Options:
                    return "HttpOptions";
                case Daibitx.DynamicApi.Attributes.HttpMethod.Trace:
                    return "HttpTrace";
                default:
                    return "HttpPost";
            }
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