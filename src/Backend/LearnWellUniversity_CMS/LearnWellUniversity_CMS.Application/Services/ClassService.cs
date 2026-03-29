using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IClassService
{
    Task<EntityCreatedResponse> CreateAsync(CreateClassRequest request);
}

public class ClassService(IUnitOfWork unitOfWork) : IClassService
{
    public async Task<EntityCreatedResponse> CreateAsync(CreateClassRequest request)
    {
        var @class = new Class
        {
            Name = request.Name,
            Description = request.Description,
        };
        await unitOfWork.Classes.AddAsync(@class);
        await unitOfWork.SaveChangesAsync();

        return new EntityCreatedResponse
        {
            Id = @class.ClassId,
            CreatedAt = @class.CreatedAt,
            CreatedBy = @class.CreatedBy
        };
    }
}