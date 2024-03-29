using Ocelot.DependencyInjection;
using Ocelot.Middleware;

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
        var corsOrigins = builder.Configuration.GetSection("CorsOrigin").Get<string[]>() ?? new string[] { string.Empty };
        options.AddPolicy("CorsPolicy",
            builder => builder.WithOrigins(corsOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
}

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSignalR();
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
app.UseWebSockets();
await app.UseOcelot();

app.Run();
