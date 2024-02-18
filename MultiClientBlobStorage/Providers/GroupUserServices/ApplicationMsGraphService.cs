using System.Text;
using Microsoft.Graph.Models;

namespace MultiClientBlobStorage.Providers.GroupUserServices;

public class ApplicationMsGraphService
{
    private readonly GraphApplicationClientService _graphApplicationClientService;

    public ApplicationMsGraphService(GraphApplicationClientService graphApplicationClientService)
    {
        _graphApplicationClientService = graphApplicationClientService;
    }

    public async Task<Group?> CreateSecurityGroupAsync(string groupName)
    {
        var graphServiceClient = _graphApplicationClientService.GetGraphClientWithClientSecretCredential();

        var formatted = RemoveSpecialCharacters(groupName);
        var groupName = $"blob-{formatted.Trim()}-{Guid.NewGuid()}".ToLower();

        var requestBody = new Group
        {
            DisplayName = groupName,
            Description = $"Security group for all users from {groupName}",
            MailEnabled = false,
            MailNickname = formatted,
            SecurityEnabled = true
        };

        var result = await graphServiceClient.Groups.PostAsync(requestBody);
        return result;
    }

    private string RemoveSpecialCharacters(string str)
    {
        var sb = new StringBuilder();
        foreach (var c in str)
        {
            if (c is >= '0' and <= '9' || c is >= 'A' and <= 'Z' || c is >= 'a' and <= 'z' || c == '.' || c == '_')
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
