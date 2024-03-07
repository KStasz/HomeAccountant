namespace Domain.Dtos.AccountingService
{
    public record ShareRegisterCreateDto
    {
        public int RegisterId { get; init; }
        public string? UserId { get; init; }
    }
}
