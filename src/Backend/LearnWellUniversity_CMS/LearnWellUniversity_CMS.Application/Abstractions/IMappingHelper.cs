namespace LearnWellUniversity_CMS.Application.Abstractions;

public interface IMappingHelper
{
    TDestination MapTo<TDestination>(object source) where TDestination : class;
    TDestination MapTo<TSource, TDestination>(TSource source) where TSource : class where TDestination : class;
    TDestination MapTo<TSource, TDestination>(TSource source, TDestination destination) where TSource : class where TDestination : class;
    IQueryable<TDestination> ProjectTo<TDestination>(IQueryable<object> query) where TDestination : class;
}
