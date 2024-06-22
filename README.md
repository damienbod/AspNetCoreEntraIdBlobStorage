![.NET](https://github.com/damienbod/AspNetCoreAzureAdAzureStorage/workflows/.NET/badge.svg)

# ASP.NET Core using Azure Blob storage 

## Blogs

[Using Blob storage from ASP.NET Core with Entra ID authentication](https://damienbod.com/2024/02/12/using-blob-storage-from-asp-net-core-with-entra-id-authentication/)

[Delegated read and application write access to blob storage using ASP.NET Core with Entra ID authentication](https://damienbod.com/2024/02/26/delegated-read-and-application-write-access-to-blob-storage-using-asp-net-core-with-entra-id-authentication/)

[Multi client blob storage access using ASP.NET Core with Entra ID authentication and RBAC](https://damienbod.com/2024/03/04/multi-client-blob-storage-access-using-asp-net-core-with-entra-id-authentication-and-rbac/)

## Delegated read/write

Secure upload and secure download. Users are authenticated using Microsoft Entra ID. The blob storage containers use Azure security groups to control the access. The upload and the download access is separated into different groups. 

![security-context](https://github.com/damienbod/AspNetCoreEntraIdBlobStorage/blob/main/Images/diagrams-delegated.png)

Assign RBAC for users or groups with role **Storage Blob Data Contributor** or **Storage Blob Data Reader** and your resource.

## Application write, delegated read

Secure upload and secure download. Users are authenticated using Microsoft Entra ID. The blob storage containers use Azure security groups to control the read access. The upload access uses the an application for the Contributor role. 

Only the application can upload files and the users or the groups can only read the files.

![security-context](https://github.com/damienbod/AspNetCoreEntraIdBlobStorage/blob/main/Images/diagrams-app-write.png)

Assign RBAC for users or groups with role **Storage Blob Data Contributor** and assign the application **Storage Blob Data Reader** and your resource.

## Multi client blob storage access using ASP.NET Core with Entra ID authentication and RBAC

Onboard different clients or organizations in an ASP.NET Core application to use separated Azure blob containers with controlled access using security groups and RBAC applied roles

![security-context](https://github.com/damienbod/AspNetCoreEntraIdBlobStorage/blob/main/Images/diagrams-app-write-multi-tenant.png)

Assign RBAC for users or groups with role **Storage Blob Data Contributor** and assign the application **Storage Blob Data Reader** and your resource.

### Old

- [Secure ME-ID User File Upload with ME-ID Storage and ASP.NET Core](https://damienbod.com/2021/02/08/secure-azure-ad-user-account-file-upload-with-azure-ad-storage-and-asp-net-core)
- [Using ME-ID groups authorization in ASP.NET Core for an Azure Blob Storage](https://damienbod.com/2021/03/01/using-azure-ad-groups-authorization-in-asp-net-core-for-an-azure-blob-storage)
- [Adding ASP.NET Core authorization for an Azure Blob Storage and ME-ID users using role assignments](https://damienbod.com/2021/02/16/adding-asp-net-core-authorization-for-an-azure-blob-storage-and-azure-ad-users-using-role-assignments)

## History

- 2024-06-22 Updated packages
- 2024-05-08 Updated packages
- 2024-03-24 Updated packages
- 2024-03-03 Updated packages
- 2024-02-09 Improved security, using Entra ID with delegated App Roles and groups
- 2024-02-07 .NET 8
- 2023-11-03 Updated packages
- 2023-08-14 Updated packages
- 2023-04-29 Updated packages
- 2023-01-22 Updated to .NET 7
- 2022-10-24 Updated packages
- 2022-06-19 Updated packages
- 2022-01-28 Updated packages, Updated to .NET 6
- 2021-07-30 Updated packages
- 2021-03-11 Updated packages

## SQL

Add-Migration "init" 
Add-Migration "UploadedBy" 

Update-Database 

## Links

https://learn.microsoft.com/en-us/azure/storage/blobs/authorize-access-azure-active-directory

https://damienbod.com/2023/01/16/implementing-secure-microsoft-graph-application-clients-in-asp-net-core/

https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction

https://github.com/AzureAD/microsoft-identity-web

https://github.com/Azure-Samples/storage-dotnet-azure-ad-msal

https://winsmarts.com/access-azure-blob-storage-with-standards-based-oauth-authentication-b10d201cbd15

https://stackoverflow.com/questions/45956935/azure-ad-roles-claims-missing-in-access-token

https://github.com/425show/b2c-appRoles

## Links Role assignments

https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles

https://blogs.aaddevsup.xyz/2020/05/using-azure-management-libraries-for-net-to-manage-azure-ad-users-groups-and-rbac-role-assignments/

https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-authentication

https://docs.microsoft.com/en-us/rest/api/authorization/role-assignment-rest-sample

