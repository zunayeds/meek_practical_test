using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Exceptions;
using LearnWellUniversity_CMS.Shared.Utilities;
using LinqKit;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IClassService
{
    Task<CreatedEntityResponse> CreateAsync(CreateClassRequest request, CancellationToken cancellationToken = default);
    Task<ClassResponse> GetByClassIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<ClassResponseBase>> GetClasssAsync(GetByFiltersBaseRequest request, CancellationToken cancellationToken = default);
    Task<ClassResponse> UpdateClassAsync(Guid id, CreateUpdateClassRequest request, CancellationToken cancellationToken = default);
    Task DeleteClassByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddRemoveStudentsInClassAsync(Guid classId, AddRemoveStudentsRequest request, CancellationToken cancellationToken = default);
    Task<List<StudentResponseBase>> GetStudentsInClassAsync(Guid classId, CancellationToken cancellationToken = default);
    Task<List<CourseResponseBase>> GetCoursesAssociatedWithClassAsync(Guid classId, CancellationToken cancellationToken = default);
}

public class ClassService(IUnitOfWork unitOfWork, ICurrentUser currentUser, IMappingHelper mappingHelper) : IClassService
{
    protected IClassRepository _classes = unitOfWork.Classes;

    public async Task<CreatedEntityResponse> CreateAsync(CreateClassRequest request, CancellationToken cancellationToken = default)
    {
        var doesExist = await _classes.DoesExistAsync(f => f.Name == request.Name, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Class>("name"));

        var @class = new Class
        {
            Name = request.Name,
            Description = request.Description,
        };
        await _classes.AddAsync(@class, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mappingHelper.MapTo<CreatedEntityResponse>(@class);
    }

    public async Task<ClassResponse> GetByClassIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var @class = await _classes.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Class>());

        return mappingHelper.MapTo<ClassResponse>(@class);
    }

    public async Task<PaginatedResponse<ClassResponseBase>> GetClasssAsync(GetByFiltersBaseRequest request, CancellationToken cancellationToken = default)
    {
        var filter = PredicateBuilder.New<Class>(true);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filter = filter.And(f => f.Name.StartsWith(request.Name));
        }

        if (currentUser.StudentId is not null)
        {
            filter = filter.And(f => f.StudentClasses.Any(w => w.StudentId == currentUser.StudentId));
        }

        return await _classes
            .GetByFiltersAsync<ClassResponseBase>(filter, request.Page, request.PageSize, cancellationToken);
    }

    public async Task<ClassResponse> UpdateClassAsync(Guid id, CreateUpdateClassRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateClassExistanceAsync(id);

        var doesExist = await _classes.DoesExistAsync(f => f.ClassId != id && f.Name == request.Name, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Class>("name"));

        var @class = await _classes.Update(id, request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mappingHelper.MapTo<ClassResponse>(@class);
    }

    public async Task DeleteClassByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await ValidateClassExistanceAsync(id);
        await _classes.DeleteByFilterAsync(f => f.ClassId == id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRemoveStudentsInClassAsync(Guid classId, AddRemoveStudentsRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateClassExistanceAsync(classId);
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        await _classes.AddRemoveStudentsAsync(classId, request.AddStudentIds, request.RemoveStudentIds, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<List<StudentResponseBase>> GetStudentsInClassAsync(Guid classId, CancellationToken cancellationToken = default)
    {
        await ValidateClassExistanceAsync(classId);
        var filter = PredicateBuilder.New<Student>(f => f.StudentClasses.Any(w => w.ClassId == classId));
        return await unitOfWork.Students.GetByFiltersAsync<StudentResponseBase>(filter, cancellationToken: cancellationToken);
    }

    public async Task<List<CourseResponseBase>> GetCoursesAssociatedWithClassAsync(Guid classId, CancellationToken cancellationToken = default)
    {
        await ValidateClassExistanceAsync(classId);
        var filter = PredicateBuilder.New<Course>(f => f.CourseClasses.Any(w => w.ClassId == classId));
        return await unitOfWork.Courses.GetByFiltersAsync<CourseResponseBase>(filter, cancellationToken: cancellationToken);
    }

    private async Task ValidateClassExistanceAsync(Guid classId)
    {
        if (!await _classes.DoesExistAsync(f => f.ClassId == classId))
        {
            throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Class>());
        }
    }
}