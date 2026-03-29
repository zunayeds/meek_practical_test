using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Exceptions;
using LearnWellUniversity_CMS.Shared.Utilities;
using LinqKit;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IClassService
{
    Task<CreatedEntityResponse> CreateAsync(CreateClassRequest request, CancellationToken cancellationToken = default);
    Task<ClassResponse> GetByClassIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ClassResponseBase>> GetClasssAsync(GetByFiltersBaseRequest request, CancellationToken cancellationToken = default);
    Task<ClassResponse> UpdateClassAsync(Guid id, CreateUpdateClassRequest request, CancellationToken cancellationToken = default);
    Task DeleteClassByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class ClassService(IUnitOfWork unitOfWork, ICurrentUser currentUser, IMappingHelper mappingHelper) : IClassService
{
    public async Task<CreatedEntityResponse> CreateAsync(CreateClassRequest request, CancellationToken cancellationToken = default)
    {
        var doesExist = await unitOfWork.Classes.DoesExistAsync(f => f.Name == request.Name, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Class>("name"));

        var @class = new Class
        {
            Name = request.Name,
            Description = request.Description,
        };
        await unitOfWork.Classes.AddAsync(@class);
        await unitOfWork.SaveChangesAsync();

        return mappingHelper.MapTo<CreatedEntityResponse>(@class);
    }

    public async Task<ClassResponse> GetByClassIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var @class = await unitOfWork.Classes.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(ErrorMessageGenerator.NotFoundErrorMessage<Class>());

        return mappingHelper.MapTo<ClassResponse>(@class);
    }

    public async Task<List<ClassResponseBase>> GetClasssAsync(GetByFiltersBaseRequest request, CancellationToken cancellationToken = default)
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

        return await unitOfWork.Classes
            .GetByFiltersAsync<ClassResponseBase>(filter, request.Page, request.PageSize, cancellationToken);
    }

    public async Task<ClassResponse> UpdateClassAsync(Guid id, CreateUpdateClassRequest request, CancellationToken cancellationToken = default)
    {
        var doesExist = await unitOfWork.Classes.DoesExistAsync(f => f.ClassId != id && f.Name == request.Name, cancellationToken);
        if (doesExist) throw new AlreadyExistException(ErrorMessageGenerator.AlreadyExistErrorMessage<Class>("name"));

        var @class = await unitOfWork.Classes.Update(id, request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mappingHelper.MapTo<ClassResponse>(@class);
    }

    public async Task DeleteClassByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await unitOfWork.Classes.DeleteByFilterAsync(f => f.ClassId == id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}