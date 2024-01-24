namespace Domain.Dtos.AccountingService
{
    public class RegisterUpdateDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
