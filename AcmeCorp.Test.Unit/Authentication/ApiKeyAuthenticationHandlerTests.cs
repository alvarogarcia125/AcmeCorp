using AcmeCorp.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Encodings.Web;

namespace AcmeCorp.Test.Unit.Authentication
{
    public class ApiKeyAuthenticationHandlerTests
    {
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _optionsMock;
        private readonly Mock<UrlEncoder> _encoderMock;
        private readonly Mock<ISystemClock> _clockMock;
        private readonly ApiKeyProvider _apiKeyProvider;
        private readonly ApiKeyAuthenticationHandler _handler;

        public ApiKeyAuthenticationHandlerTests()
        {
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _optionsMock = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _encoderMock = new Mock<UrlEncoder>();
            _clockMock = new Mock<ISystemClock>();
            _apiKeyProvider = new ApiKeyProvider("test-api-key");

            _optionsMock.Setup(o => o.CurrentValue).Returns(new AuthenticationSchemeOptions());

            _handler = new ApiKeyAuthenticationHandler(
                _optionsMock.Object,
                _loggerFactoryMock.Object,
                _encoderMock.Object,
                _clockMock.Object,
                _apiKeyProvider
            );
        }

        private async Task InitializeHandler(HttpContext context)
        {
            await _handler.InitializeAsync(new AuthenticationScheme("ApiKey", null, typeof(ApiKeyAuthenticationHandler)), context);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldAuthenticate_WhenApiKeyIsValid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["X-API-KEY"] = "test-api-key";
            await InitializeHandler(context);

            // Act
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("ApiKeyUser", result.Principal.Identity.Name);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldNotAuthenticate_WhenApiKeyIsMissing()
        {
            // Arrange
            var context = new DefaultHttpContext();
            await InitializeHandler(context);

            // Act
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Principal);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldNotAuthenticate_WhenApiKeyIsInvalid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["X-API-KEY"] = "invalid-api-key";
            await InitializeHandler(context);

            // Act
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid API Key provided.", result.Failure.Message);
        }
    }
}
