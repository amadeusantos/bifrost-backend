using Bifrost.Core.Domain.Enum;

namespace Bifrost.Core.Domain.User;

public class User: UserMinimal
{
    public Course.Course? Course { get; set; }
}