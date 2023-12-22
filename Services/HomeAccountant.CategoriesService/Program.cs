using HomeAccountant.CategoriesService.Data;
using HomeAccountant.CategoriesService.Service;
using Microsoft.EntityFrameworkCore;
using JwtAuthenticationManager;
using Microsoft.OpenApi.Models;
using Domain;
using Domain.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Accounting Service",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.ConfigureDbContext();
builder.Services.AddScoped<IAccountingService, AccountingService>();
builder.Services.AddJwtAuthentication();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<AuthorizedHttpClient>(
    c => c.BaseAddress = new Uri(builder.Configuration["AccountingServiceBaseAdress"] ?? throw new ArgumentNullException("AccountingServiceBaseAdress")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

if (builder.Environment.IsProduction())
    app.PrepareDatabase<ApplicationDbContext>();

app.Run();
