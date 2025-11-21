using Daibitx.AspNetCore.DynamicApi.Abstraction.Attributes;
using Daibitx.AspNetCore.DynamicApi.Abstraction.Interfaces;

namespace WebApplication1.IServices
{
    [RoutePrefix("/api/user/test")]
    public interface IUserService : IDynamicController
    {
        Task<UserDto> GetUserAsync(long id);
        Task<List<UserDto>> GetUsersAsync(string name);
        Task CreateUserAsync(CreateUserDto dto);
        Task UpdateUserAsync(long id, UpdateUserDto dto);
        Task DeleteUserAsync(long id);
    }

    public class UserDto { public long Id { get; set; } public string Name { get; set; } }
    public class CreateUserDto { public string Name { get; set; } }
    public class UpdateUserDto { public string Name { get; set; } }
}

