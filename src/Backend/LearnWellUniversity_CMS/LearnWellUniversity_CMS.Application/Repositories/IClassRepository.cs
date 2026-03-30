using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Domain.Models;

namespace LearnWellUniversity_CMS.Application.Repositories;

public interface IClassRepository : IRepository<Class>
{
    Task AddRemoveStudentsAsync(Guid classId, List<Guid> addStudentIds, List<Guid> removeStudentIds, CancellationToken cancellationToken = default);
}
