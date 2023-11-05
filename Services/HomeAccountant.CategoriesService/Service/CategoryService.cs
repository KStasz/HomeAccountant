using HomeAccountant.CategoriesService.Data;
using HomeAccountant.CategoriesService.Model;

namespace HomeAccountant.CategoriesService.Service
{
    public class CategoryService : ICategoriesService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Add(CategoryModel entity)
        {
            _applicationDbContext.Categories.Add(entity);
        }

        public void Delete(CategoryModel entity)
        {
            _applicationDbContext.Categories.Remove(entity);
        }

        public CategoryModel? Get(int id)
        {
            return _applicationDbContext.Categories.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<CategoryModel> GetAll()
        {
            return _applicationDbContext.Categories;
        }

        public void Update(CategoryModel entity)
        {
            _applicationDbContext.Categories.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
