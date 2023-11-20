using Microsoft.AspNetCore.Mvc;

namespace HomeAccountant.AccountingService.Controllers
{
    public class ServiceControllerBase  : ControllerBase
    {
        internal string? GetUserId() => this.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
    }
}
