namespace BetterMembership.Utils
{
    using System.Collections.Generic;
    using System.Configuration;

    internal interface ISqlQueryBuilder
    {
        string FindUsersByEmailQuery { get; }

        string FindUsersByNameQuery { get; }

        string GetAllUsersQuery { get; }

        string GetUserByIdQuery { get; }

        string GetUserNameByEmail { get; }

        string GetUserProfile { get; }

        string GetUserQuery { get; }

        string UnlockUser { get; }

        string UpdateUserEmail { get; }

        string UpdateUserMembership { get; }

        string UpdateUserProfile(IEnumerable<SettingsPropertyValue> properties, string userName, out object[] values);
    }
}