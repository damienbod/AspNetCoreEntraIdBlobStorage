namespace GraphClientCrendentials;

public class AadGraphSdkApplicationClient
{
    private readonly GraphApplicationClientService _graphService;

    public AadGraphSdkApplicationClient(IConfiguration configuration, GraphApplicationClientService graphService)
    {
        _graphService = graphService;
    }

    public async Task<long?> GetUsersAsync()
    {
        var graphServiceClient = _graphService.GetGraphClientWithClientSecretCredential();

        var users = await graphServiceClient.Users
            .GetAsync();

        return users!.Value!.Count;
    }
}
