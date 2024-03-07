using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.DTOs.Friends;
using HomeAccountant.Core.DTOs.Identity;
using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAccountant.Core.Services
{
    public static class TypeMapperConfigurator
    {
        public static void RegisterMappers(this IServiceCollection services)
        {
            services.AddScoped<ITypeMapper<TokenAuthenticationModel, LoginResponseDto>, TokenAuthenticationMapper>();
            services.AddScoped<ITypeMapper<LoginResponseModel, LoginResponseDto>, LoginResponseDtoToLoginResponseModelMapper>();
            services.AddScoped<ITypeMapper<RegisterCreateDto, RegisterModel>, RegisterModelToRegisterCreateDtoMapper>();
            services.AddScoped<ITypeMapper<RegisterModel, RegisterReadDto>, RegisterReadDtoToRegisterModelMapper>();
            services.AddScoped<ITypeMapper<LoginResponseModel, TokenAuthenticationModel>, TokenAuthenticationModelToLoginResponseModelMapper>();
            services.AddScoped<ITypeMapper<BillingPeriodCreateDto, BillingPeriodModel>, BillingPeriodModelToBillingPeriodCreateDtoMapper>();
            services.AddScoped<ITypeMapper<BillingPeriodModel, BillingPeriodReadDto>, BillingPeriodReadDtoToBillingPeriodModelMapper>();
            services.AddScoped<ITypeMapper<EntryModel, EntryReadDto>, EntryReadDtoToEntryModelMapper>();
            services.AddScoped<ITypeMapper<CategoryModel, CategoryReadDto>, CategoryReadDtoToCategoryModelMapper>();
            services.AddScoped<ITypeMapper<BillingPeriodStatisticModel, BillingPeriodStatisticDto>, BillingPeriodStatisticDtoToBillingPeriodStatisticModelMapper>();
            services.AddScoped<ITypeMapper<EntriesStatisticChartDataModel, EntriesStatisticChartDataDto>, EntriesStatisticChartDataDtoToEntriesStatisticChartDataModelMapper>();
            services.AddScoped<ITypeMapper<EntryCreateDto, EntryModel>, EntryModelToEntryCreateDtoMapper>();
            services.AddScoped<ITypeMapper<EntryUpdateDto, EntryModel>, EntryModelToEntryUpdateDtoMapper>();
            services.AddScoped<ITypeMapper<CategoryCreateDto, CategoryModel>, CategoryModelToCategoryCreateDtoMapper>();
            services.AddScoped<ITypeMapper<CategoryUpdateDto, CategoryModel>, CategoryModelToCategoryUpdateDtoMapper>();
            services.AddScoped<ITypeMapper<FriendshipModel, FriendshipReadDto>, FriendshipReadDtoToFriendshipModelMapper>();
            services.AddScoped<ITypeMapper<UserModel, UserModelReadDto>, UserModelReadDtoToUserModelMapper>();
        }
    }
}
