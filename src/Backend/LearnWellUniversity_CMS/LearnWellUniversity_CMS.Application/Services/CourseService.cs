using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;

namespace LearnWellUniversity_CMS.Application.Services;

public interface ICourseService
{
    Task<EntityCreatedResponse> CreateAsync(CreateCourseRequest request);
}

public class CourseService(IUnitOfWork unitOfWork) : ICourseService
{
    public async Task<EntityCreatedResponse> CreateAsync(CreateCourseRequest request)
    {
        var course = new Course
        {
            Name = request.Name,
            Description = request.Description,
        };
        await unitOfWork.Courses.AddAsync(course);
        await unitOfWork.SaveChangesAsync();

        return new EntityCreatedResponse
        {
            Id = course.CourseId,
            CreatedAt = course.CreatedAt,
            CreatedBy = course.CreatedBy
        };
    }
}