using HomeAccountant.CategoriesService.Model;

namespace HomeAccountant.CategoriesService.Service
{
    public interface ICategoriesService : IReaderService<CategoryModel>, IWriterService<CategoryModel>
    {
        Task SaveChangesAsync();
    }
}
