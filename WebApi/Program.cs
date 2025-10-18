using WebApi.Extensions;
using WebApi.Constants;
using WebApi.ServerMail;
using WebApi.Structs;
using WebApi.ContextDB;
using WebApi.Managers;
using WebApi.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var allowSpecificOrigins = "_allowSpecificOrigins";
var connectionString = builder.Configuration.GetConnectionString("connName");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7252"
                                           , "https://saa.servicenow.app.br"
                                           , "https://budget.servicenow.app.br"
                                           , "https://operation.servicenow.app.br"
                                           , "https://servicenow.app.br"
                                           , "https://web.servicenow.app.br");
                          policy.WithMethods("PATCH", "GET", "POST", "PUT");
                          policy.WithHeaders("Authorization", "Content-Type");
                      });
});

builder.Services.AddDbContext<WebApiContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Audience"],
        ValidIssuer = builder.Configuration["Issuer"]
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("POLICY_MAIL_SERVICE", policy => policy.RequireClaim("WEB_USER", "allow-send-mail"));

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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o JWT como: Bearer {Seu token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{ Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme,Id = "Bearer"}},
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddControllers();

builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(allowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/version/numbers/{number}", async (int number) =>
{
    return Results.Ok(await Versions.GetInfo(number));
})
.Produces<VersionView>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetVersion")
.WithTags("Version");

app.MapPatch("/mail/send/organizations/{organizationCode}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "POLICY_MAIL_SERVICE")] async (
    string organizationCode,
    WebApiContext webApiContext,
    IEmailSender emailSender,
    IMemoryCache memoryCache,
    EmailStruct email) =>
{
    try
    {
        MailInfoViewModel mailInfo;
        
        if (memoryCache.TryGetValue(organizationCode, out MailInfoViewModel? cachemailInfo))
        {
            mailInfo = cachemailInfo!;
        }
        else
        {
            var mailInfoManager = new MailInfoManager(webApiContext);
            mailInfo = await mailInfoManager.Get(organizationCode) ?? throw new Exception($"ERROR: Configuração de e-mail não encontrada para a organização informada. Ref.: {organizationCode}");

            memoryCache.Set(organizationCode, mailInfo, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(10),
                SlidingExpiration = TimeSpan.FromHours(8)
            });
        }

        await emailSender.SenderEmailAsync(mailInfo, email.From, email.To, email.ToName
            , email.Subject, email.Message, email.MailAttachments, email.MessagePriority);
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
.WithName("PatchMail")
.WithTags("Mail")
.RequireCors(allowSpecificOrigins);

app.MapPost("/mail/send/create-or-replace", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "POLICY_MAIL_SERVICE")] async (
    WebApiContext webApiContext,
    IEmailSender emailSender,
    IMemoryCache memoryCache,
    MailInfoViewModel mailInfo) =>
{
    try
    {
        if (memoryCache.TryGetValue(mailInfo.OrganizationCode!, out MailInfoViewModel? cachemailInfo))
            memoryCache.Remove(mailInfo.OrganizationCode!);
    
        var mailInfoManager = new MailInfoManager(webApiContext);
        var operationReturn = await mailInfoManager.NewOrReplace(mailInfo);

        return operationReturn.IsSuccess ? Results.Ok(new ReturnMessage("200", operationReturn.FormatMessage)) : 
                                           Results.BadRequest(new ReturnMessage("400", operationReturn.FormatMessage));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new ReturnMessage("500", ex.MessageAll()));
    }
})
.ProducesValidationProblem()
.Produces<ReturnMessage>(StatusCodes.Status200OK)
.Produces<ReturnMessage>(StatusCodes.Status400BadRequest)
.WithName("PostMail")
.WithTags("Mail")
.RequireCors(allowSpecificOrigins);

app.Run();