namespace BetterMembership.Facades
{
    internal interface IWebSecurityFacade
    {
        void InitializeDatabaseConnection(
            string connectionStringName, 
            string userTableName, 
            string userIdColumn, 
            string userNameColumn, 
            bool autoCreateTables);
    }
}