using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.Services
{
    public interface IAlert
    {
        Task ShowAlert(string message, AlertType type);
    }
}
