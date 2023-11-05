namespace HomeAccountant.CategoriesService.Service
{
    public interface IReaderService<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? Get(int id);
    }
}
