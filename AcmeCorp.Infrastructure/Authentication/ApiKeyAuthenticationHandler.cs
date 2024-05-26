using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AcmeCorp.Infrastructure.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string ApiKeyHeaderName = "X-API-KEY";
        private readonly ApiKeyProvider _apiKeyProvider;
        private readonly ILogger<ApiKeyAuthenticationHandler> _logger;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            ApiKeyProvider apiKeyProvider)
            : base(options, loggerFactory, encoder, clock)
        {
            _apiKeyProvider = apiKeyProvider;
            _logger = loggerFactory.CreateLogger<ApiKeyAuthenticationHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                _logger.LogWarning("API Key header not found.");
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                _logger.LogWarning("API Key header is empty.");
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (providedApiKey == _apiKeyProvider.ApiKey)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, "ApiKeyUser")
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                _logger.LogInformation("API Key authenticated successfully.");
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                _logger.LogWarning("Invalid API Key provided.");
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key provided."));
            }
        }
    }
}
