namespace Infraestructure.Azure;

public sealed class AzureCosmosDbConfig
{
    public string Endpoint { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
