using Domain.Dtos.CategoryService;
using Domain.Dtos.IdentityPlatform;

namespace Domain.Dtos.AccountingService
{
    public class EntryReadDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required CategoryReadDto? Category { get; set; }
        public required RegisterReadDto Register { get; set; }
        public decimal Price { get; set; }
        public required string Creator { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
