using Microsoft.EntityFrameworkCore;

namespace Domain.Services
{
    public interface IRepository<U, T> : IReaderService<T>, IWriterService<T> where T : class where U : DbContext
    {
        Task SaveChangesAsync();
    }
}
