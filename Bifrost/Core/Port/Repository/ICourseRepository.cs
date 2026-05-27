using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Course;

namespace Bifrost.Core.Port.Repository;

public interface ICourseRepository : IRepository<Course>
{
    Task<Pagination<Course>> GetCourses(int page, int size);
    Task<bool> ExistsByCode(string code);
    Task<bool> ExistsByCodeExcludingId(string code, Guid excludedId);
}
