![.NET](https://github.com/damienbod/AspNetCoreAzureAdAzureStorage/workflows/.NET/badge.svg)

# ASP.NET Core Azure Storage Secure Files

Assign RBAC for user with role **Storage Blob Data Contributor** and your resource.

## Blogs

<ul>
	<li><a href="https://damienbod.com/2021/02/08/secure-azure-ad-user-account-file-upload-with-azure-ad-storage-and-asp-net-core/">Secure Azure AD User File Upload with Azure AD Storage and ASP.NET Core</a></li>
	<li><a href="https://damienbod.com/2021/02/16/adding-asp-net-core-authorization-for-an-azure-blob-storage-and-azure-ad-users-using-role-assignments/">Adding ASP.NET Core authorization for an Azure Blob Storage and Azure AD users using role assignments</a></li>
	<li><a href="https://damienbod.com/2021/03/01/using-azure-ad-groups-authorization-in-asp-net-core-for-an-azure-blob-storage/">Using Azure AD groups authorization in ASP.NET Core for an Azure Blob Storage</a></li>
</ul>

## History

2021-07-30 Updated packages

2021-03-11 Updated packages

## SQL

Add-Migration "init" 
Add-Migration "UploadedBy" 

Update-Database 

# Links

https://github.com/Azure-Samples/storage-dotnet-azure-ad-msal

https://winsmarts.com/access-azure-blob-storage-with-standards-based-oauth-authentication-b10d201cbd15

https://stackoverflow.com/questions/45956935/azure-ad-roles-claims-missing-in-access-token

https://github.com/AzureAD/microsoft-identity-web

https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction

https://github.com/425show/b2c-appRoles

## Links Role assignments

https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles

https://blogs.aaddevsup.xyz/2020/05/using-azure-management-libraries-for-net-to-manage-azure-ad-users-groups-and-rbac-role-assignments/

https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Authorization/roleAssignments?api-version=2015-07-01

https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-authentication

https://docs.microsoft.com/en-us/rest/api/authorization/role-assignment-rest-sample

