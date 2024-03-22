using HomeAccountant;
using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.Services;
using HomeAccountant.Core.ViewModels;
using HomeAccountant.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
bool isUsingLocal = GetValueResponsibleForLocalEnvironment();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddScoped<IJsCodeExecutor, JsCodeExecutor>();
builder.Services.AddSingleton<ITokenStorageAccessor, TokenStorageAccessor>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IEntryService, EntryService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IModalService, ModalService>();
builder.Services.AddScoped<IJwtTokenParser, JwtTokenParser>();
builder.Services.AddScoped<IBillingPeriodService, BillingPeriodService>();
builder.Services.RegisterMappers();

builder.Services.AddHttpClient("UnauhorizedHttpClient",
    client => client.BaseAddress = new Uri(GetBaseAddress(isUsingLocal)));

builder.Services.AddHttpClient<AuthorizableHttpClient>(
    client => client.BaseAddress = new Uri(GetBaseAddress(isUsingLocal)));
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<RegisterViewModel>();
builder.Services.AddTransient<EntryViewModel>();
builder.Services.AddScoped<CategoriesViewModel>();
builder.Services.AddScoped<BillingPeriodViewModel>();
builder.Services.AddScoped<BillingPeriodChartViewModel>();
builder.Services.AddTransient<FriendsPanelViewModel>();
builder.Services.AddScoped<IMemoryStorage, MemoryStorage>();
builder.Services.AddScoped<IPubSubService, PubSubService>();
builder.Services.RegisterSignalRService<IFriendsRealTimeService, FriendsRealTimeService>(isUsingLocal ? "FriendsHubAddress_local" : "FriendsHubAddress");




await builder.Build().RunAsync();

string GetBaseAddress(bool isUsingLocal)
{
    var address = isUsingLocal ? builder.Configuration["APIBaseAddress_local"] : builder.Configuration["APIBaseAddress"];

    return address ?? throw new ArgumentNullException();
}

bool GetValueResponsibleForLocalEnvironment()
{
    return builder.Configuration.GetValue<bool>("UseLocalBackend");
}