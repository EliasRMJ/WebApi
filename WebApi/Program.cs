using Microsoft.OpenApi.Models;
using WebApi.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "ServiceNow (OPERATION)",
        Description = "APIs integradas de gerenciamento de execu��o de servi�os.",
        Contact = new OpenApiContact() { Name = "Devesk", Email = "contact@servicenow.app.br", Url = new Uri("https://api.servicenow.app.br") }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/version/number/{number}", async (int number) =>
{
    return await Versions.GetNumber(number);
})
.Produces<int>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetVersion")
.WithTags("Version");

app.Run();