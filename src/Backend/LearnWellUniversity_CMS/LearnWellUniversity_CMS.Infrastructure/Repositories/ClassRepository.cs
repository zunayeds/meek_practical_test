using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class ClassRepository(AppDbContext dbContext, ICurrentUser currentUser, IMappingHelper mappingHelper) : Repository<Class>(dbContext, currentUser, mappingHelper), IClassRepository
{

}
