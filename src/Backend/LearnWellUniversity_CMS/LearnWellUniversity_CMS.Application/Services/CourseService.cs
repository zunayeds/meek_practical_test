using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Exceptions;
using LearnWellUniversity_CMS.Shared.Utilities;
using LinqKit;

namespace LearnWellUniversity_CMS.Application.Services;

public interface ICourseService
{
    Task<CreatedEntityResponse> CreateAsync(CreateUpdateCourseRequest request, CancellationToken cancellationToken = default);
    Task<CourseResponse> GetByCourseIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<CourseResponseBase>> GetCoursesAsync(GetByFiltersBaseRequest request, CancellationToken cancellationToken = default);
    Task<CourseResponse> UpdateCourseAsync(Guid id, CreateUpdateCourseRequest request, CancellationToken cancellationToken = default);
    Task DeleteCourseByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddRemoveStudentsInCourseAsync(Guid courseId, AddRemoveStudentsRequest request, CancellationToken cancellationToken = default);
    Task<List<StudentResponseBase>> GetStudentsInCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<List<ClassResponseBase>> GetClassesInCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task AddRemoveClassesInCourseAsync(Guid courseId, AddRemoveClassessRequest request, CancellationToken cancellationToken = default);
}

public class CourseService(IUnitOfWork unitOfWork, ICurrentUser currentUser, IMappingHelper mappingHelper) : ICourseService
{
    protected ICourseRepository _courses = unitOfWork.Courses;

    public async Task<CreatedEntityResponse> CreateAsync(CreateUpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var doesExist = await _courses.DoesExistAsync(f => f.Name == request.Name, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Course>("name"));

        var course = new Course
        {
            Name = request.Name,
            Description = request.Description,
        };
        await _courses.AddAsync(course, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mappingHelper.MapTo<CreatedEntityResponse>(course);
    }

    public async Task<CourseResponse> GetByCourseIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await _courses.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Course>());

        return mappingHelper.MapTo<CourseResponse>(course);
    }

    public async Task<List<CourseResponseBase>> GetCoursesAsync(GetByFiltersBaseRequest request, CancellationToken cancellationToken = default)
    {
        var filter = PredicateBuilder.New<Course>(true);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filter = filter.And(f => f.Name.StartsWith(request.Name));
        }

        if (currentUser.StudentId is not null)
        {
            filter = filter.And(f => f.StudentCourses.Any(w => w.StudentId == currentUser.StudentId));
        }

        return await _courses
            .GetByFiltersAsync<CourseResponseBase>(filter, request.Page, request.PageSize, cancellationToken);
    }

    public async Task<CourseResponse> UpdateCourseAsync(Guid id, CreateUpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateCourseExistanceAsync(id);

        var doesExist = await _courses.DoesExistAsync(f => f.CourseId != id && f.Name == request.Name, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Course>("name"));

        var course = await _courses.Update(id, request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mappingHelper.MapTo<CourseResponse>(course);
    }

    public async Task DeleteCourseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await ValidateCourseExistanceAsync(id);
        await _courses.DeleteByFilterAsync(f => f.CourseId == id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRemoveStudentsInCourseAsync(Guid courseId, AddRemoveStudentsRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateCourseExistanceAsync(courseId);
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        await _courses.AddRemoveStudentsAsync(courseId, request.AddStudentIds, request.RemoveStudentIds, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<List<StudentResponseBase>> GetStudentsInCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        await ValidateCourseExistanceAsync(courseId);
        var filter = PredicateBuilder.New<Student>(f => f.StudentCourses.Any(w => w.CourseId == courseId));
        return await unitOfWork.Students.GetByFiltersAsync<StudentResponseBase>(filter, cancellationToken: cancellationToken);
    }

    public async Task<List<ClassResponseBase>> GetClassesInCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        await ValidateCourseExistanceAsync(courseId);
        var filter = PredicateBuilder.New<Class>(f => f.CourseClasses.Any(w => w.CourseId == courseId));
        return await unitOfWork.Classes.GetByFiltersAsync<ClassResponseBase>(filter, cancellationToken: cancellationToken);
    }

    public async Task AddRemoveClassesInCourseAsync(Guid courseId, AddRemoveClassessRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateCourseExistanceAsync(courseId);
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        await _courses.AddRemoveClassesAsync(courseId, request.AddClassIds, request.RemoveClassIds, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task ValidateCourseExistanceAsync(Guid courseId)
    {
        if (!await _courses.DoesExistAsync(f => f.CourseId == courseId))
        {
            throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Course>());
        }
    }
}