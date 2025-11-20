using Daibitx.DynamicApi.Abstraction.Attributes;
using Daibitx.DynamicApi.Abstraction.Interfaces;

namespace DemoApp.Services;

/// <summary>
/// 用于测试 DynamicApiGenerator 的示例接口
/// </summary>
[RoutePrefix("/api/sample")]
[ApiExplorerSettings(false, "Sample Service")]
public interface ISampleService : IDynamicController
{
    /// <summary>
    /// GET 示例，无参数
    /// </summary>
    Task<string> GetWelcomeMessage();

    /// <summary>
    /// GET 示例，URL 参数 + Query 参数
    /// </summary>
    /// <param name="id">路径参数</param>
    /// <param name="keyword">查询参数</param>
    [HttpMethod(DynamicMethod.Get)]
    Task<string> GetItemAsync(int id, string keyword);

    /// <summary>
    /// POST 示例，Body 参数
    /// </summary>
    /// <param name="model">请求模型</param>
    [HttpMethod(DynamicMethod.Post)]
    Task<bool> CreateItemAsync(SampleCreateModel model);

    /// <summary>
    /// PUT 示例，路径参数 + Body
    /// </summary>
    [HttpMethod(DynamicMethod.Put)]
    Task<bool> UpdateItemAsync(int id, SampleUpdateModel model);

    /// <summary>
    /// DELETE 示例，带可选参数
    /// </summary>
    /// <param name="id">路径参数</param>
    /// <param name="force">可选参数</param>
    [HttpMethod(DynamicMethod.Delete)]
    Task<bool> DeleteItemAsync(int id, bool force = false);

    /// <summary>
    /// 获取列表的示例
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    [HttpMethod(DynamicMethod.Get)]
    Task<IEnumerable<SampleListItem>> GetListAsync(int pageIndex = 1, int pageSize = 10);
}


// ---------- 示例模型 ----------
public class SampleCreateModel
{
    public string Name { get; set; }
    public int Count { get; set; }
}

public class SampleUpdateModel
{
    public string Name { get; set; }
    public bool Enabled { get; set; }
}

public class SampleListItem
{
    public int Id { get; set; }
    public string Name { get; set; }
}
