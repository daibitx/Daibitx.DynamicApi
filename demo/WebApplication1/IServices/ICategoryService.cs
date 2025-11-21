using Daibitx.AspNetCore.DynamicApi.Abstraction.Attributes;
using Daibitx.AspNetCore.DynamicApi.Abstraction.Interfaces;

namespace WebApplication1.IServices;

/// <summary>
/// 分类管理服务接口
/// </summary>
[RoutePrefix("/api/categories")]
[ApiExplorerSettings(true, "Category Service")]
public interface ICategoryService : IDynamicController
{
    /// <summary>
    /// 获取分类树形结构
    /// </summary>
    [HttpMethod(DynamicMethod.Get)]
    Task<List<CategoryTreeDto>> GetCategoryTreeAsync();

    /// <summary>
    /// 根据ID获取分类详情
    /// </summary>
    [HttpMethod(DynamicMethod.Get)]
    Task<CategoryDto?> GetCategoryByIdAsync(long id);

    /// <summary>
    /// 创建分类
    /// </summary>
    [HttpMethod(DynamicMethod.Post)]
    Task<long> CreateCategoryAsync(CreateCategoryDto dto);

    /// <summary>
    /// 更新分类信息
    /// </summary>
    [HttpMethod(DynamicMethod.Put)]
    Task<bool> UpdateCategoryAsync(long id, UpdateCategoryDto dto);

    /// <summary>
    /// 删除分类
    /// </summary>
    [HttpMethod(DynamicMethod.Delete)]
    Task<bool> DeleteCategoryAsync(long id);

    /// <summary>
    /// 获取分类下的产品数量
    /// </summary>
    [HttpMethod(DynamicMethod.Get)]
    Task<int> GetCategoryProductCountAsync(long id);

    /// <summary>
    /// 更新分类排序
    /// </summary>
    [HttpMethod(DynamicMethod.Put)]
    Task<bool> UpdateCategorySortAsync(long id, int sortOrder);
}

// 分类相关DTO
public class CategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateTime { get; set; }
}

public class CategoryTreeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public List<CategoryTreeDto> Children { get; set; } = new();
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? ParentId { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; }
}