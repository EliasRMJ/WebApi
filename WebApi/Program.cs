using WebApi.Extensions;
using WebApi.Constants;
using WebApi.ServerMail;
using WebApi.Structs;
using System.Collections.Generic;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

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

builder.Services.AddMinio(configureClient => configureClient
                .WithEndpoint("quiron-dev-quiron-storage.2qexrs.easypanel.host", 443)
                .WithCredentials("17PEqnp93fEo9mCp6fNG", "YbJgTjF5FF4nQlDfShywUI624JK0i2QSAaVCwpGW")
                .WithSSL(true)
                .Build());

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

app.MapGet("/minio/buckets/{bucketID}/prefixs/{prefix}", async (string bucketID, string prefix, IMinioClient minioClient) =>
{
    bool found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketID));
    if (!found)
        throw new Exception($"Erro. Ref.: {bucketID}");

    var observable = minioClient.ListObjectsEnumAsync(new ListObjectsArgs()
        .WithBucket(bucketID)
        .WithPrefix(Uri.UnescapeDataString(prefix))
        .WithRecursive(true)
        .WithVersions(true));


    return Results.Ok(observable);
})
.Produces<VersionView>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetList")
.WithTags("Minio");

app.MapGet("/minio/buckets/{bucketID}/objects/{*key}", async (
    string bucketID, string key, IMinioClient minioClient) =>
{
    var memoryStream = new MemoryStream();
    var obj = await minioClient.GetObjectAsync(new GetObjectArgs()
                                            .WithBucket(bucketID)
                                            .WithObject(Uri.UnescapeDataString(key))
                                            .WithCallbackStream((stream) => { stream.CopyTo(memoryStream); }));

    memoryStream.Position = 0;

    var contentType = "application/octet-stream";
    var fileName = Path.GetFileName(Uri.UnescapeDataString(key));

    return Results.File(memoryStream, contentType, fileName);
})
.Produces<VersionView>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetObject")
.WithTags("Minio");

app.MapDelete("/minio/buckets/{bucketID}/keys/{key}", async (
    string bucketID, string key, IMinioClient minioClient) =>
{
    bool found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketID));
    if (!found)
        throw new Exception($"Erro. Ref.: {bucketID}");

    await minioClient.RemoveObjectAsync(new RemoveObjectArgs()
        .WithBucket(bucketID)
        .WithObject(Uri.UnescapeDataString(key)));

    return Results.Ok("removido!!!");
})
.Produces<VersionView>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("DelObject")
.WithTags("Minio");

app.Run();