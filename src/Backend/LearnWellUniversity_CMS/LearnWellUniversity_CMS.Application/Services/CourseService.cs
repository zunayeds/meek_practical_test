using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;
using LinqKit;
using System.Data.Entity.Core;

namespace LearnWellUniversity_CMS.Application.Services;

public interface ICourseService
{
    Task<EntityCreatedResponse> CreateAsync(CreateUpdateCourseRequest request, CancellationToken cancellationToken = default);
    Task<CourseResponse> GetByCourseByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<CourseResponseBase>> GetCoursesAsync(GetByFiltersBaseRequest request, CancellationToken cancellationToken = default);
    Task<CourseResponse> UpdateCourseAsync(Guid id, CreateUpdateCourseRequest request, CancellationToken cancellationToken = default);
    Task DeleteCourseByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class CourseService(IUnitOfWork unitOfWork, ICurrentUser currentUser, IMappingHelper mappingHelper) : ICourseService
{
    public async Task<EntityCreatedResponse> CreateAsync(CreateUpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var course = new Course
        {
            Name = request.Name,
            Description = request.Description,
        };
        await unitOfWork.Courses.AddAsync(course, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EntityCreatedResponse
        {
            Id = course.CourseId,
            CreatedAt = course.CreatedAt,
            CreatedBy = course.CreatedBy
        };
    }

    public async Task<CourseResponse> GetByCourseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await unitOfWork.Courses.GetByIdAsync(id, cancellationToken) ?? throw new ObjectNotFoundException($"Cannot find course with Id '{id}'");

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

        return await unitOfWork.Courses
            .GetByFiltersAsync<CourseResponseBase>(filter, request.Page, request.PageSize, cancellationToken);
    }

    public async Task<CourseResponse> UpdateCourseAsync(Guid id, CreateUpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var course = await unitOfWork.Courses.Update(id, request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mappingHelper.MapTo<CourseResponse>(course);
    }

    public async Task DeleteCourseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await unitOfWork.Courses.DeleteByFilterAsync(f => f.CourseId == id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}