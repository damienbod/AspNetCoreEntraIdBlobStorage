namespace GraphClientCrendentials;

public class MicrosoftGraphApplicationClient
{
    private readonly GraphApplicationClientService _graphService;

    public MicrosoftGraphApplicationClient(GraphApplicationClientService graphService)
    {
        _graphService = graphService;
    }

    /// <summary>
    /// Storage Blob Data Reader: ID: 2a2b9908-6ea1-4ae2-8e65-a410df84e7d1
    /// Application.ReadWrite.All AppRoleAssignment.ReadWrite.All
    /// https://cloud.google.com/bigquery/docs/omni-azure-create-connection#microsoft-rest-api
    /// </summary>
    public async Task StorageBlobDataReaderRoleAssignment(string groupId, string resourceId)
    {
        // Storage Blob Data Reader: 2a2b9908-6ea1-4ae2-8e65-a410df84e7d1
        var graphServiceClient = _graphService.GetGraphClientWithClientSecretCredential();

        var users = await graphServiceClient.Users
            .GetAsync();

    }
}
