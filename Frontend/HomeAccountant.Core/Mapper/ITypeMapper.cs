namespace HomeAccountant.Core.Mapper
{
    public interface ITypeMapper<TDestination, TSource>
    {
        public TDestination Map(TSource? value);
    }
}
