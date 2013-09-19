namespace BetterMembership.Utils
{
    using System.Collections.Generic;
    using System.Configuration;

    internal interface ISqlQueryBuilder
    {
        string DeleteMembershipUser { get; }

        string DeleteOAuthMembershipUser { get; }

        string DeleteProfile { get; }

        string DeleteUserInRoles { get; }

        string FindUsersByEmail { get; }

        string FindUsersByName { get; }

        string GetAllUsers { get; }

        string GetUser { get; }

        string GetUserById { get; }

        string GetUserNameByEmail { get; }

        string GetUserProfile { get; }

        string UnlockUser { get; }

        string UpdateUserEmail { get; }

        string UpdateUserMembership { get; }

        string UpdateUserProfile(IEnumerable<SettingsPropertyValue> properties, string userName, out object[] values);
    }
}