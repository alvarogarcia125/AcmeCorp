namespace AcmeCorp.Infrastructure.Authentication
{
    public class ApiKeyProvider
    {
        public string ApiKey { get; }

        public ApiKeyProvider(string apiKey)
        {
            ApiKey = apiKey;
        }
    }
}
