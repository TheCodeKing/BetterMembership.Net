namespace BetterMembership.Facades
{
    using CuttingEdge.Conditions;

    using WebMatrix.WebData;

    internal sealed class WebSecurityFacade : IWebSecurityFacade
    {
        public void InitializeDatabaseConnection(
            string connectionStringName, 
            string userTableName, 
            string userIdColumn, 
            string userNameColumn, 
            bool autoCreateTables)
        {
            Condition.Requires(connectionStringName, "connectionStringName").IsNotNullOrWhiteSpace();
            Condition.Requires(userTableName, "userTableName").IsNotNullOrWhiteSpace();
            Condition.Requires(userIdColumn, "userIdColumn").IsNotNullOrWhiteSpace();
            Condition.Requires(userNameColumn, "userNameColumn").IsNotNullOrWhiteSpace();

            WebSecurity.InitializeDatabaseConnection(
                connectionStringName, userTableName, userIdColumn, userNameColumn, autoCreateTables);
        }
    }
}