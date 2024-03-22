using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAccountant.Core.Services
{
    public static class SignalRServiceConfiguration
    {
        public static void RegisterSignalRService<TService, TImplementation>(this IServiceCollection serviceCollection, string hubConfigName)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddKeyedTransient<ISignalRHubConnection, SignalRHubConnection>(
                nameof(TService),
                (s, k) =>
                {
                    var config = s.GetRequiredService<IConfiguration>();
                    var address = config[hubConfigName] ?? throw new ArgumentNullException(nameof(hubConfigName));
                    var hubConnection = new HubConnectionBuilder().WithUrl(address).Build();
                    var hubConnectionStateGetter = new HubConnectionStateGetter(hubConnection);
                    var hubConnectionSenderAsync = new HubConnectionSender(hubConnection);
                    var hubConnectionConfigurator = new HubConnectionConfigurator(hubConnection);

                    return new SignalRHubConnection(
                        hubConnection,
                        hubConnectionSenderAsync,
                        hubConnectionStateGetter,
                        hubConnectionConfigurator);
                });

            serviceCollection.AddTransient<TService, TImplementation>((s) =>
            {
                var service = s.GetRequiredKeyedService<ISignalRHubConnection>(nameof(TService));

                var serviceName = service
                .GetType()
                .GetInterfaces()
                .FirstOrDefault(
                    x => x.Name
                    .ToLower()
                    .Contains(service.GetType().Name.ToLower()))?.Name
                    .ToString();

                var ctorsParameters = typeof(TImplementation)
                .GetConstructors()
                .SelectMany(x => x.GetParameters())
                .Where(x => x.ParameterType.Name != serviceName)
                .Select(x => x.ParameterType)
                .Select(x => s.GetRequiredService(x))
                .ToList();

                ctorsParameters.Insert(0, service);

#pragma warning disable CS8603
#pragma warning disable CS8600
                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), ctorsParameters.ToArray());
#pragma warning restore CS8603
#pragma warning restore CS8600
            });

        }
    }
}
