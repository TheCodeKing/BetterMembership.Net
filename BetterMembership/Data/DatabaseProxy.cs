namespace BetterMembership.Data
{
    using System;
    using System.Collections.Generic;

    using CuttingEdge.Conditions;

    using WebMatrix.Data;

    internal sealed class DatabaseProxy : IDatabase
    {
        private readonly Database database;

        public DatabaseProxy(Database database)
        {
            Condition.Requires(database, "database").IsNotNull();

            this.database = database;
        }

        ~DatabaseProxy()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public int Execute(string commandText, params object[] args)
        {
            Condition.Requires(commandText, "commandText").IsNotNullOrWhiteSpace();

            return this.database.Execute(commandText, args);
        }

        public IEnumerable<dynamic> Query(string commandText, params object[] parameters)
        {
            Condition.Requires(commandText, "commandText").IsNotNullOrWhiteSpace();

            return this.database.Query(commandText, parameters);
        }

        public dynamic QuerySingle(string commandText, params object[] args)
        {
            Condition.Requires(commandText, "commandText").IsNotNullOrWhiteSpace();

            return this.database.QuerySingle(commandText, args);
        }

        public dynamic QueryValue(string commandText, params object[] parameters)
        {
            Condition.Requires(commandText, "commandText").IsNotNullOrWhiteSpace();

            return this.database.QueryValue(commandText, parameters);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.database.Dispose();
            }
        }
    }
}