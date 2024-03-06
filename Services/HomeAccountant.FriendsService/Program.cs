using HomeAccountant.FriendsService.Data;
using HomeAccountant.FriendsService.Service;
using Microsoft.EntityFrameworkCore;
using JwtAuthenticationManager;
using HomeAccountant.FriendsService.Config;
using Domain;
using Domain.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IAuthorizationTokenProvider, AuthorizationTokenProvider>();
builder.Services.ConfigureDbContext();

builder.Services.RegisterHttpClientServices(builder.Configuration);

builder.Services.AddJwtAuthentication();

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
