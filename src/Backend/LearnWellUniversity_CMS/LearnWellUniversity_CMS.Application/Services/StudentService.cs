using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;
using LearnWellUniversity_CMS.Shared.Exceptions;
using LearnWellUniversity_CMS.Shared.Utilities;
using LinqKit;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IStudentService
{
    Task<CreateStudentResponse> CreateAsync(CreateUpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse> GetByStudentIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<StudentResponseBase>> GetStudentsAsync(GetStudentByFiltersRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse> UpdateStudentAsync(Guid id, CreateUpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task DeleteStudentByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class StudentService(IUnitOfWork unitOfWork, IUserService userService, ICurrentUser currentUser, IMappingHelper mappingHelper) : IStudentService
{
    public async Task<CreateStudentResponse> CreateAsync(CreateUpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var doesExist = await unitOfWork.Students.DoesExistAsync(f => f.EmailAddress == request.EmailAddress, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Student>("email address"));

        doesExist = await unitOfWork.Students.DoesExistAsync(f => f.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Student>("phone number"));

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
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);

            var result = mappingHelper.MapTo<CreateStudentResponse>(student);
            result.Password = password;

            return result;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<StudentResponse> GetByStudentIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.Students.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"Cannot find Student with Id '{id}'");

        return mappingHelper.MapTo<StudentResponse>(student);
    }

    public async Task<List<StudentResponseBase>> GetStudentsAsync(GetStudentByFiltersRequest request, CancellationToken cancellationToken = default)
    {
        var filter = PredicateBuilder.New<Student>(true);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filter = filter.And(f => f.FirstName.StartsWith(request.Name) || f.LastName.StartsWith(request.Name));
        }
        if (!string.IsNullOrWhiteSpace(request.EmailAddress))
        {
            filter = filter.And(f => f.EmailAddress.Contains(request.EmailAddress));
        }
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            filter = filter.And(f => f.PhoneNumber.Contains(request.PhoneNumber));
        }

        return await unitOfWork.Students
            .GetByFiltersAsync<StudentResponseBase>(filter, request.Page, request.PageSize, cancellationToken);
    }

    public async Task<StudentResponse> UpdateStudentAsync(Guid id, CreateUpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var doesExist = await unitOfWork.Students.DoesExistAsync(f => f.StudentId != id && f.EmailAddress == request.EmailAddress, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Student>("email address"));

        doesExist = await unitOfWork.Students.DoesExistAsync(f => f.StudentId != id && f.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Student>("phone number"));

        var student = await unitOfWork.Students.Update(id, request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mappingHelper.MapTo<StudentResponse>(student);
    }

    public async Task DeleteStudentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await unitOfWork.Students.DeleteByFilterAsync(f => f.StudentId == id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}