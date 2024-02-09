using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreAzureStorageGroups;

public class StorageBlobDataContributorRoleHandler : AuthorizationHandler<StorageBlobDataContributorRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        StorageBlobDataContributorRoleRequirement requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        //  "6705345e-c37e-4f7a-b2d9-e2f43e029524" // StorageContributorsAzureADfiles

        var spIdUserGroup = context.User.Claims.FirstOrDefault(t => t.Type == "groups" &&
        t.Value == "6705345e-c37e-4f7a-b2d9-e2f43e029524");

        if (spIdUserGroup != null)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}