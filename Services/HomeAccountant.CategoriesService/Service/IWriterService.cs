namespace HomeAccountant.CategoriesService.Service
{
    public interface IWriterService<T>
    {
        void Update(T entity);
        void Add(T entity);
        void Delete(T entity);
    }
}
