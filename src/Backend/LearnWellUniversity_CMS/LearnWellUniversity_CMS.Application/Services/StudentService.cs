using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IStudentService
{
    Task<CreateStudentResponse> CreateAsync(CreateStudentRequest request);
}

public class StudentService(IUnitOfWork unitOfWork, IUserService userService, ICurrentUser currentUser) : IStudentService
{
    public async Task<CreateStudentResponse> CreateAsync(CreateStudentRequest request)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.EmailAddress,
                Email = request.EmailAddress,
                EmailConfirmed = true,
                PhoneNumber = request.PhoneNumber,
                PhoneNumberConfirmed = true,
                CreatedBy = currentUser.UserId,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            var (userId, password) = await userService.CreateWithRoleAsync(user, Roles.Student, null, true);

            var student = new Student
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                EmailAddress = request.EmailAddress,
                PhoneNumber = request.PhoneNumber,
                UserId = userId
            };
            await unitOfWork.Students.AddAsync(student);
            await unitOfWork.SaveChangesAsync();

            await unitOfWork.CommitAsync();

            return new CreateStudentResponse
            {
                Id = student.StudentId,
                CreatedAt = student.CreatedAt,
                CreatedBy = student.CreatedBy,
                Password = password
            };
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}