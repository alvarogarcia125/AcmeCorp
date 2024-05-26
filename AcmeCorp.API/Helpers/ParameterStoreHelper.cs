using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement;

namespace AcmeCorp.API.Helpers
{
    public class ParameterStoreHelper
    {
        public static async Task<string> RetrieveApiKeyFromParameterStore(IConfiguration configuration, string environment, string parameter)
        {
            var awsOptions = configuration.GetAWSOptions();
            var ssmClient = awsOptions.CreateServiceClient<IAmazonSimpleSystemsManagement>();

            var parameterName = configuration["ParameterStore:" + parameter];
            parameterName = parameterName.Replace("{environment}", environment.ToLower());

            var request = new GetParameterRequest
            {
                Name = parameterName,
                WithDecryption = true
            };

            var response = await ssmClient.GetParameterAsync(request);
            return response.Parameter.Value;
        }
    }
}
