using LearnWellUniversity_CMS.API.Extensions.Infrastructure;
using LearnWellUniversity_CMS.Application.Abstractions;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices();

builder.ConfigureStructuralLogging();

builder.Services.ConfigureExceptionHandler();

builder.Services.AddControllers();

builder.Services.ConfigureSwaggerDoc();

builder.ConfigureDbContext();

builder.ConfigureAuthentication();
builder.Services.ConfigureAuthorization();

builder.Services.AddAutoMapper(cfg => { }, Assembly.GetAssembly(typeof(ICurrentUser)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.UseSwaggerDocWithUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.SeedInitialDataAsync();

app.Run();
