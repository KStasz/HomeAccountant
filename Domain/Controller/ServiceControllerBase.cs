using Microsoft.AspNetCore.Mvc;

namespace Domain.Controller
{
    public class ServiceControllerBase : ControllerBase
    {
        public string? UserId => User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
    }
}
