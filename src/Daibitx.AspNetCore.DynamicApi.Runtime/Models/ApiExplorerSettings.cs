namespace Daibitx.AspNetCore.DynamicApi.Runtime.Models;
public class ApiExplorerSettings
{
    /// <summary>
    /// When set to true, the API will be ignored in the API documentation.
    /// </summary>
    public bool IgnoreApi { get; set; }

    /// <summary>
    /// Get or set the group name for the API description.
    /// </summary>
    public string GroupName { get; set; }
}
