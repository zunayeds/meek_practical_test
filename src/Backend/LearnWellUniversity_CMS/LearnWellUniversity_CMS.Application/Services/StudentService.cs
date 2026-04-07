using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;
using LearnWellUniversity_CMS.Shared.Exceptions;
using LearnWellUniversity_CMS.Shared.Utilities;
using LinqKit;
using Microsoft.Extensions.Logging;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IStudentService
{
    Task<CreateStudentResponse> CreateAsync(CreateUpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse> GetByStudentIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<StudentResponseBase>> GetStudentsAsync(GetStudentByFiltersRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse> UpdateStudentAsync(Guid id, CreateUpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task DeleteStudentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<string>> GetOtherStudentNamesByClassIdAsync(Guid classId, CancellationToken cancellationToken = default);
    Task<List<StudentClassResponse>> GetClassesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<List<StudentClassResponse>> GetClassesAsync(CancellationToken cancellationToken = default);
}

public class StudentService(IUnitOfWork unitOfWork, IUserService userService, ICurrentUser currentUser, IMappingHelper mappingHelper, ILogger<StudentService> logger) : IStudentService
{
    protected IStudentRepository _students = unitOfWork.Students;

    public async Task<CreateStudentResponse> CreateAsync(CreateUpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var doesExist = await _students.DoesExistAsync(f => f.EmailAddress == request.EmailAddress, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Student>("email address"));

        doesExist = await _students.DoesExistAsync(f => f.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Student>("phone number"));

        await unitOfWork.BeginTransactionAsync(cancellationToken);

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

            logger.LogInformation("Creating user with 'Student' role for student '{FirstName} {LastName}'", request.FirstName, request.LastName);
            var (userId, password) = await userService.CreateWithRoleAsync(user, Roles.Student, null, true);
            logger.LogInformation("Created user with 'Student' role for student '{FirstName} {LastName}'", request.FirstName, request.LastName);

            var student = new Student
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                EmailAddress = request.EmailAddress,
                PhoneNumber = request.PhoneNumber,
                UserId = userId
            };
            await _students.AddAsync(student, cancellationToken);
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
        var student = await _students.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Student>());

        return mappingHelper.MapTo<StudentResponse>(student);
    }

    public async Task<PaginatedResponse<StudentResponseBase>> GetStudentsAsync(GetStudentByFiltersRequest request, CancellationToken cancellationToken = default)
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

        return await _students
            .GetByFiltersAsync<StudentResponseBase>(filter, request.Page, request.PageSize, cancellationToken);
    }

    public async Task<StudentResponse> UpdateStudentAsync(Guid id, CreateUpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateStudentExistanceAsync(id);

        var doesExist = await _students.DoesExistAsync(f => f.StudentId != id && f.EmailAddress == request.EmailAddress, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Student>("email address"));

        doesExist = await _students.DoesExistAsync(f => f.StudentId != id && f.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Student>("phone number"));

        var student = await _students.Update(id, request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mappingHelper.MapTo<StudentResponse>(student);
    }

    public async Task DeleteStudentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await ValidateStudentExistanceAsync(id);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var userId = await _students.GetUserIdAsync(id, cancellationToken);
        await _students.DeleteByFilterAsync(f => f.StudentId == id, cancellationToken);
        logger.LogInformation("Deleting user for student with id {id}", id);
        await userService.DeleteUserByIdAsync(userId);
        logger.LogWarning("Deleted user for student with id {id}", id);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<List<string>> GetOtherStudentNamesByClassIdAsync(Guid classId, CancellationToken cancellationToken = default)
    {
        return await _students.GetOtherStudentNamesByClassIdAsync(classId, cancellationToken);
    }

    public async Task<List<StudentClassResponse>> GetClassesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        await ValidateStudentExistanceAsync(studentId);
        return await _students.GetClassesAsync(studentId, cancellationToken);
    }

    public async Task<List<StudentClassResponse>> GetClassesAsync(CancellationToken cancellationToken = default)
    {
        var studentId = currentUser.StudentId ?? throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Student>());
        await ValidateStudentExistanceAsync(studentId);
        return await _students.GetClassesAsync(studentId, cancellationToken);
    }

    private async Task ValidateStudentExistanceAsync(Guid studentId)
    {
        if (!await _students.DoesExistAsync(f => f.StudentId == studentId))
        {
            throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Student>());
        }
    }
}