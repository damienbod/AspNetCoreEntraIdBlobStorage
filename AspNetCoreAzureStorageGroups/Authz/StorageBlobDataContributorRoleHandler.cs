using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorageGroups
{
    public class StorageBlobDataContributorRoleHandler : AuthorizationHandler<StorageBlobDataContributorRoleRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            StorageBlobDataContributorRoleRequirement requirement
        )
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (requirement == null)
                throw new ArgumentNullException(nameof(requirement));

            // TODO add auth using group

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}