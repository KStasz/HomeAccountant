using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.DTOs.Register;

namespace HomeAccountant.Core.DTOs.Entry
{
    public class EntryReadDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required CategoryReadDto Category { get; set; }
        public required BillingPeriodReadDto Period { get; set; }
        public decimal Price { get; set; }
        public required string Creator { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
