using WebApi.Extensions;
using WebApi.Constants;
using WebApi.ServerMail;
using WebApi.Structs;

var builder = WebApplication.CreateBuilder(args);
var allowSpecificOrigins = "_allowSpecificOrigins";

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
                      policy => {
                          policy.WithOrigins("https://localhost:7252"
                                           , "https://saa.servicenow.app.br"
                                           , "https://budget.servicenow.app.br"
                                           , "https://operation.servicenow.app.br"
                                           , "https://servicenow.app.br"
                                           , "https://web.servicenow.app.br");
                          policy.WithMethods("GET", "POST");
                          policy.WithHeaders("Authorization", "Content-Type");
                      });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Version = "v1",
        Title = "ServiceNow (WebApi)",
        Description = "APIs integradas de gerenciamento de execução de serviços.",
        Contact = new() { Name = "ServiceNow", Email = "contact@servicenow.app.br", Url = new Uri("https://api.servicenow.app.br") }
    });
});

builder.Services.AddSingleton<IEmailSender, EmailSender>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseCors(allowSpecificOrigins);

app.MapGet("/version/numbers/{number}", async (int number) =>
{
    return Results.Ok(await Versions.GetInfo(number));
})
.Produces<VersionView>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetVersion")
.WithTags("Version");

app.MapPost("/mail/send", async (
      IConfiguration configurationUser,
      IEmailSender emailSender,
      EmailStruct email) =>
{
    try
    {
        await emailSender.SenderEmailAsync(email.From, email.To, email.ToName
            , email.Subject, email.Message);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new ReturnMessage("400", ex.MessageAll()));
    }

    return Results.Ok(new ReturnMessage("200", "E-mail enviado com sucesso!"));
})
.ProducesValidationProblem()
.Produces<ReturnMessage>(StatusCodes.Status200OK)
.Produces<ReturnMessage>(StatusCodes.Status400BadRequest)
.WithName("PostMail")
.WithTags("Mail")
.RequireCors(allowSpecificOrigins);

app.Run();