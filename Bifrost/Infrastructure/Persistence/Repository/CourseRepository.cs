using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Course;
using Bifrost.Core.Port.Repository;
using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Repository;

public class CourseRepository(ApplicationDbContext applicationDbContext) : RepositoryBase<CourseEntity, Course>(applicationDbContext), ICourseRepository
{
    protected override Course EntityToDomain(CourseEntity entity)
    {
        return new Course {Id =  entity.Id, Name = entity.Name, Code =  entity.Code};
    }

    protected override CourseEntity DomainToEntity(Course domain)
    {
        return new CourseEntity {Id = domain.Id, Name = domain.Name, Code = domain.Code};
    }

    public async Task<Pagination<Course>> GetCourses(int page, int size)
    {
        int skip = (page - 1) * size;
        int total = await dbSet.CountAsync();
        var courses = await dbSet.Skip(skip).Take(size).ToArrayAsync();
        return new Pagination<Course>(page, size, total, courses.Select(EntityToDomain).ToArray());
    }

    public async Task<bool> ExistsByCode(string code)
    {
        return await dbSet.AnyAsync(c => c.Code == code);
    }

    public async Task<bool> ExistsByCodeExcludingId(string code, Guid excludedId)
    {
        return await dbSet.AnyAsync(e => e.Code == code && e.Id != excludedId);
    }
}