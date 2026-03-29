using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Domain.Models;

namespace LearnWellUniversity_CMS.Application.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    public Task<Guid> GetIdByUserIdAsync(Guid userId);
}
