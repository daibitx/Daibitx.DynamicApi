using System;

namespace Daibitx.DynamicApi.Abstraction.Attributes
{
    /// <summary>
    /// API 文档生成行为
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class ApiExplorerSettingsAttribute : Attribute
    {
        /// <summary>
        /// 是否在 API 文档中隐藏
        /// </summary>
        public bool IgnoreApi { get; }

        /// <summary>
        /// API 分组名称
        /// </summary>
        public string GroupName { get; }

        public ApiExplorerSettingsAttribute(bool ignoreApi, string groupName)
        {
            IgnoreApi = ignoreApi;
            GroupName = groupName;
        }
    }
}
