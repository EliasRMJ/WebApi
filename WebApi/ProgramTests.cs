using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using WebApi;
using WebApi.ServerMail;
using WebApi.Structs;
using Xunit;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetVersion_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/version/numbers/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var version = await response.Content.ReadFromJsonAsync<VersionView>();
        Assert.NotNull(version);
    }

    [Fact]
    public async Task PostMail_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = new EmailStruct
        {
            From = "test@example.com",
            To = "recipient@example.com",
            ToName = "Recipient",
            Subject = "Test Subject",
            Message = "Test Message",
            UserSsl = true
        };

        // Act
        var response = await client.PostAsJsonAsync("/mail/send", email);

        // Assert
        response.EnsureSuccessStatusCode();
        var returnMessage = await response.Content.ReadFromJsonAsync<ReturnMessage>();
        Assert.NotNull(returnMessage);
        Assert.Equal("200", returnMessage.Code);
    }

    [Fact]
    public async Task PostMail_ReturnsBadRequest_OnException()
    {
        // Arrange
        var mockEmailSender = new Mock<IEmailSender>();
        mockEmailSender.Setup(sender => sender.SenderEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                       .ThrowsAsync(new Exception("Test Exception"));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(mockEmailSender.Object);
            });
        }).CreateClient();

        var email = new EmailStruct
        {
            From = "test@example.com",
            To = "recipient@example.com",
            ToName = "Recipient",
            Subject = "Test Subject",
            Message = "Test Message",
            UserSsl = true
        };

        // Act
        var response = await client.PostAsJsonAsync("/mail/send", email);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var returnMessage = await response.Content.ReadFromJsonAsync<ReturnMessage>();
        Assert.NotNull(returnMessage);
        Assert.Equal("400", returnMessage.Code);
    }
}
