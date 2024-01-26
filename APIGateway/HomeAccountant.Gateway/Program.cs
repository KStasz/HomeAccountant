using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var useCors = builder.Configuration.GetSection("UseCors").Get<bool>();

Console.WriteLine($"--> UseCors: {useCors}");

if (useCors)
{
    builder.Services.AddCors(options =>
    {
        var corsOrigin = builder.Configuration["CorsOrigin"] ?? string.Empty;
        options.AddPolicy("CorsPolicy",
            builder => builder.WithOrigins(corsOrigin)
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
}

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (useCors)
    app.UseCors("CorsPolicy");

app.Map("/swagger/v1/swagger.json", b =>
{
    b.Run(async x =>
    {
        var json = File.ReadAllText("swagger.json");
        await x.Response.WriteAsync(json);
    });
});


//Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(opt => opt.SupportedSubmitMethods());
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.UseOcelot();

app.Run();
