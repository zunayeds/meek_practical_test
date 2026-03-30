using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Domain.Models;

namespace LearnWellUniversity_CMS.Application.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task AddRemoveStudentsAsync(Guid courseId, List<Guid> addStudentIds, List<Guid> removeStudentIds, CancellationToken cancellationToken = default);
    Task AddRemoveClassesAsync(Guid courseId, List<Guid> addClassIds, List<Guid> removeClassIds, CancellationToken cancellationToken = default);
}
