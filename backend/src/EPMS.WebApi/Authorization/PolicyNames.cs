namespace EPMS.WebApi.Authorization;

/// <summary>Nama policy authorization berbasis role (RBAC), dipakai di [Authorize(Policy = ...)].</summary>
public static class PolicyNames
{
    public const string AdminOnly = "AdminOnly";

    public const string AdminOrManager = "AdminOrManager";

    public const string AnyAuthenticatedUser = "AnyAuthenticatedUser";
}
