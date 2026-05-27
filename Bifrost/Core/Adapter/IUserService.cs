using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;

namespace Bifrost.Core.Adapter;

public interface IUserService
{
    public Task<User> CreateUser(UserCreateDto userCreateDto);
    
    public Task<User> GetUser(Guid id);
    
    public Task<Pagination<User>> GetUsers(int page, int size, UserProfileEnum? profile, Guid? courseId);
    
    public Task<User> UpdateUser(Guid id, UserUpdateDto userUpdateDto);

    public Task DeleteUser(Guid id);
}