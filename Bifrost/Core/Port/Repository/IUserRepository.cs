using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;

namespace Bifrost.Core.Port.Repository;

public interface IUserRepository: IRepository<User>
{
    public Task<User?> FindByEmail(string email);
    public Task<User?> FindByGoogleId(string googleId);
    public Task<bool> EmailExists(string email);
    public Task<Pagination<User>> GetUsers(int page, int size, UserProfileEnum? profile, Guid? courseId);
    public Task<List<User>> FindManyUsers(List<Guid> userIds, UserProfileEnum profile, Guid courseId);
}