using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Course;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Port;
using Bifrost.Core.Port.Repository;
using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Repository;

public class UserRepository(ApplicationDbContext applicationDbContext) :RepositoryBase<UserEntity, User>(applicationDbContext), IUserRepository
{

    public async Task<User?> FindByEmail(string email)
    {
        UserEntity? userEntity = await dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        return userEntity is not null ? EntityToDomain(userEntity) : null;
    }

    public async Task<bool> EmailExists(string email)
    {
        return await dbSet.AnyAsync(u => u.Email == email);
    }

    public async Task<Pagination<User>> GetUsers(int page, int size, UserProfileEnum? profile, Guid? courseId)
    {
        int skip = (page - 1) * size;
        IQueryable<UserEntity> query = dbSet;
        if (profile != null) query = query.Where(u => u.Profile == profile);
        if (courseId != null) query = query.Where(u => u.CourseId == courseId);
        int total = await query.CountAsync();
        UserEntity[] result = await query.Skip(skip).Take(size).OrderBy(u => u.FullName).ToArrayAsync();
        return new Pagination<User>(page, size, total, result.Select(EntityToDomain).ToArray());
    }

    public async Task<List<User>> FindManyUsers(List<Guid> userIds, UserProfileEnum profile, Guid courseId)
    { var users=  await dbSet
            .Where(u => userIds.Contains(u.Id) && u.Profile == profile && u.CourseId == courseId)
            .ToListAsync();
    return users.Select(EntityToDomain).ToList();
    }

    protected override User EntityToDomain(UserEntity entity)
    {
        return new User
        {
            Id = entity.Id, 
            Email = entity.Email, 
            FullName = entity.FullName, 
            CourseId = entity.CourseId, 
            IsAdmin = entity.IsAdmin, 
            GoogleOpenid = entity.GoogleOpenid,
            Profile =  entity.Profile,
            Course = entity.Course != null
                ? new () {Id = entity.Course.Id, Code =  entity.Course.Code, Name = entity.Course.Name}
                : null,
        };
    }

    protected override UserEntity DomainToEntity(User domain)
    {
        return new UserEntity
        {
            Id = domain.Id,
            Email = domain.Email,
            FullName = domain.FullName,
            CourseId = domain.CourseId,
            IsAdmin = domain.IsAdmin,
            GoogleOpenid = domain.GoogleOpenid,
            Profile = domain.Profile,
        };
    }
}