using Microsoft.AspNetCore.Mvc;

namespace Domain.Controller
{
    public class ServiceControllerBase : ControllerBase
    {
        public string? GetUserId() => this.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
    }
}
