using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core;

public static class ServiceResponseExtensions
{
    public static async Task<T?> GetValue<T>(this ServiceResponse<T> response, IAlert? alert, CancellationToken cancellationToken = default)
    {
        if (!response.Result)
        {
            if (alert is null)
                return default;
            
            await alert.ShowAlertAsync(response.Errors.JoinToString(), AlertType.Danger, cancellationToken);
            
            return default;
        }

        return response.Value;
    }
}
