using AutoMapper;
using AutoMapper.QueryableExtensions;
using LearnWellUniversity_CMS.Application.Abstractions;

namespace LearnWellUniversity_CMS.Infrastructure.Services;

public class MappingHelper(IMapper mapper) : IMappingHelper
{
    public TDestination MapTo<TDestination>(object source) where TDestination : class
        => mapper.Map<TDestination>(source);

    public TDestination MapTo<TSource, TDestination>(TSource source) where TSource : class where TDestination : class
        => mapper.Map<TDestination>(source);

    public TDestination MapTo<TSource, TDestination>(TSource source, TDestination destination) where TSource : class where TDestination : class
        => mapper.Map(source, destination);

    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable<object> query) where TDestination : class
        => query.ProjectTo<TDestination>(mapper.ConfigurationProvider);
}
