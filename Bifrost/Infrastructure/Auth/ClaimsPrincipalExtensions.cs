using System.Security.Claims;
using Bifrost.Core.Domain.Enum;

namespace Bifrost.Infrastructure.Auth;

public static class ClaimsPrincipalExtensions
{
    public static string GetEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email)!;

    public static Guid GetId(this ClaimsPrincipal user) =>
        new (user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static UserProfileEnum GetProfile(this ClaimsPrincipal user) =>
        Enum.Parse<UserProfileEnum>(user.FindFirstValue(ClaimTypes.Role)!);

    public static bool GetIsAdmin(this ClaimsPrincipal user) =>
        bool.Parse(user.FindFirstValue("IsAdmin") ?? "False");

    public static Guid? GetCourseId(this ClaimsPrincipal user) => 
        user.FindFirstValue("CourseId") == null || user.FindFirstValue("CourseId") == "" 
            ? new Guid(user.FindFirstValue("CourseId")!): null;
}
