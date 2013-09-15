namespace BetterMembership.Utils
{
    internal interface ISqlQueryBuilder
    {
        string FindUsersByEmailQuery { get; }

        string FindUsersByNameQuery { get; }

        string GetAllUsersQuery { get; }

        string GetUserByIdQuery { get; }

        string GetUserNameByEmail { get; }

        string GetUserQuery { get; }

        string UnlockUser { get; }

        string UpdateUserMembership { get; }

        string UpdateUserProfile { get; }
    }
}