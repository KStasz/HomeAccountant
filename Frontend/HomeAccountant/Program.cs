using HomeAccountant;
using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
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
builder.Services.AddSingleton<ITokenStorageAccessor, TokenStorageAccessor>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IEntryService, EntryService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IModalService, ModalService>();
builder.Services.AddScoped<IJwtTokenParser, JwtTokenParser>();
builder.Services.AddScoped<IBillingPeriodService, BillingPeriodService>();
builder.Services.AddScoped<ITypeMapper<TokenAuthenticationModel, LoginResponseDto>, TokenAuthenticationMapper>();
builder.Services.AddScoped<ITypeMapper<LoginResponseModel, LoginResponseDto>, LoginResponseDtoToLoginResponseModelMapper>();
builder.Services.AddScoped<ITypeMapper<RegisterCreateDto, RegisterModel>, RegisterModelToRegisterCreateDtoMapper>();
builder.Services.AddScoped<ITypeMapper<RegisterModel, RegisterReadDto>, RegisterReadDtoToRegisterModelMapper>();
builder.Services.AddScoped<ITypeMapper<LoginResponseModel, TokenAuthenticationModel>, TokenAuthenticationModelToLoginResponseModelMapper>();
builder.Services.AddScoped<ITypeMapper<BillingPeriodCreateDto, BillingPeriodModel>, BillingPeriodModelToBillingPeriodCreateDtoMapper>();
builder.Services.AddScoped<ITypeMapper<BillingPeriodModel, BillingPeriodReadDto>, BillingPeriodReadDtoToBillingPeriodModelMapper>();
builder.Services.AddScoped<ITypeMapper<EntryModel, EntryReadDto>, EntryReadDtoToEntryModelMapper>();
builder.Services.AddScoped<ITypeMapper<CategoryModel, CategoryReadDto>, CategoryReadDtoToCategoryModelMapper>();
builder.Services.AddScoped<ITypeMapper<BillingPeriodStatisticModel, BillingPeriodStatisticDto>, BillingPeriodStatisticDtoToBillingPeriodStatisticModelMapper>();
builder.Services.AddScoped<ITypeMapper<EntriesStatisticChartDataModel, EntriesStatisticChartDataDto>, EntriesStatisticChartDataDtoToEntriesStatisticChartDataModelMapper>();
builder.Services.AddScoped<ITypeMapper<EntryCreateDto, EntryModel>, EntryModelToEntryCreateDtoMapper>();
builder.Services.AddScoped<ITypeMapper<EntryUpdateDto, EntryModel>, EntryModelToEntryUpdateDtoMapper>();
builder.Services.AddScoped<ITypeMapper<CategoryCreateDto, CategoryModel>, CategoryModelToCategoryCreateDtoMapper>();
builder.Services.AddScoped<ITypeMapper<CategoryUpdateDto, CategoryModel>, CategoryModelToCategoryUpdateDtoMapper>();

builder.Services.AddHttpClient("UnauhorizedHttpClient",
    client => client.BaseAddress = new Uri(GetBaseAddress()));

builder.Services.AddHttpClient<AuthorizableHttpClient>(
    client => client.BaseAddress = new Uri(GetBaseAddress()));
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<RegisterViewModel>();
builder.Services.AddTransient<EntryViewModel>();
builder.Services.AddScoped<CategoriesViewModel>();
builder.Services.AddScoped<BillingPeriodViewModel>();
builder.Services.AddScoped<BillingPeriodChartViewModel>();
builder.Services.AddScoped<IMemoryStorage, MemoryStorage>();
builder.Services.AddScoped<IPubSubService, PubSubService>();

await builder.Build().RunAsync();

string GetBaseAddress()
{
    var useLocalBackend = builder.Configuration.GetValue<bool>("UseLocalBackend");
    var address = useLocalBackend ? builder.Configuration["APIBaseAddress_local"] : builder.Configuration["APIBaseAddress"];

    return address ?? throw new ArgumentNullException();
}