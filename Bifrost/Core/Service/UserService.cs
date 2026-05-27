using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Exception;
using Bifrost.Core.Exception.Course;
using Bifrost.Core.Exception.User;
using Bifrost.Core.Port;
using Bifrost.Core.Port.Repository;

namespace Bifrost.Core.Service;

public class UserService(IUserRepository userRepository, ICourseRepository courseRepository) : IUserService
{
    public async Task<User> CreateUser(UserCreateDto userCreateDto)
    {
        if (!await courseRepository.IdExists(userCreateDto.CourseId))
        {
            throw new CourseNotFoundException();
        }
        
        if (await userRepository.EmailExists(userCreateDto.Email)) throw new UserEmailAlreadyExistsException();

        User user = new User
        {
            Email = userCreateDto.Email, 
            CourseId = userCreateDto.CourseId,
            FullName =  userCreateDto.FullName,
            Profile =  userCreateDto.Profile,
            IsAdmin = userCreateDto.IsAdmin,
        };
        return await userRepository.Add(user);
    }

    public async Task<User> GetUser(Guid id)
    {
        User? user = await userRepository.FindById(id);
        return user ?? throw new UserNotFoundException();
    }

    public async Task<Pagination<User>> GetUsers(int page, int size, UserProfileEnum? profile, Guid? courseId)
    {
        return await userRepository.GetUsers(page, size, profile, courseId);
    }

    public async Task<User> UpdateUser(Guid id, UserUpdateDto userUpdateDto)
    {
        User? user = await userRepository.FindById(id);
        if (user is null) throw new UserNotFoundException();

        if (userUpdateDto.CourseId.HasValue && !await courseRepository.IdExists(userUpdateDto.CourseId.Value))
            throw new CourseNotFoundException();

        user.FullName = userUpdateDto.FullName;
        user.Profile = userUpdateDto.Profile;
        user.IsAdmin = userUpdateDto.IsAdmin ?? user.IsAdmin;
        user.CourseId = userUpdateDto.CourseId ?? user.CourseId;
        return await userRepository.Update(user);
    }

    public async Task DeleteUser(Guid id)
    {
        if (!await userRepository.IdExists(id)) throw new UserNotFoundException();

        await userRepository.DeleteById(id);
    }
}