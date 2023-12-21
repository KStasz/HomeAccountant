using AutoMapper;
using HomeAccountant;
using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.Services;
using HomeAccountant.Core.Storage;
using HomeAccountant.Core.ViewModels;
using HomeAccountant.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddScoped<IJsCodeExecutor, JsCodeExecutor>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<ILocalStorage, LocalStorage>();
builder.Services.AddSingleton<ITokenStorageAccessor, TokenStorageAccessor>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IEntryService, EntryService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IModalService, ModalService>();
builder.Services.AddScoped<IJwtTokenParser, JwtTokenParser>();
builder.Services.AddScoped<IBillingPeriodService, BillingPeriodService>();

builder.Services.AddHttpClient("UnauhorizedHttpClient", 
    client => client.BaseAddress = new Uri(GetBaseAddress()));

builder.Services.AddHttpClient<AuthorizableHttpClient>(
    client => client.BaseAddress = new Uri(GetBaseAddress()));
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<RegisterViewModel>();
builder.Services.AddTransient<EntryViewModel>();
builder.Services.AddScoped<CategoriesViewModel>();
builder.Services.AddScoped<BillingPeriodViewModel>();

await builder.Build().RunAsync();

string GetBaseAddress()
{
    var useLocalBackend = builder.Configuration.GetValue<bool>("UseLocalBackend");
    var address = useLocalBackend ? builder.Configuration["APIBaseAddress_local"] : builder.Configuration["APIBaseAddress"];

    return address ?? throw new ArgumentNullException();
}