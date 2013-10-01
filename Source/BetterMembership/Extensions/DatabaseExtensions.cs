namespace BetterMembership.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    using BetterMembership.Data;

    internal static class DatabaseExtensions
    {
        public static bool AddColumnToTable(
            this IDatabase db, 
            string userTableName, 
            string userEmailColumn, 
            SqlDbType dataType, 
            int maxEmailLength, 
            bool allowNull, 
            bool isUnique)
        {
            var buffer =
                new StringBuilder(@"ALTER TABLE [").Append(userTableName)
                                                   .Append("] ADD [")
                                                   .Append(userEmailColumn)
                                                   .Append("] ")
                                                   .Append(Convert.ToString(dataType))
                                                   .Append("(")
                                                   .Append(maxEmailLength)
                                                   .Append(")");

            if (allowNull)
            {
                buffer.Append(" NOT");
            }

            buffer.Append(" NULL");

            if (isUnique)
            {
                buffer.Append(" UNIQUE");
            }

            try
            {
                db.Execute(buffer.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ColumnExists(this IDatabase db, string tableName, string columnName)
        {
            var query =
                db.QuerySingle(
                    @"SELECT * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @0 and COLUMN_NAME = @1", 
                    tableName, 
                    columnName);
            return query != null;
        }

        public static IList<string> GetColumnsForTable(this IDatabase db, string tableName)
        {
            var columns = new List<string>();
            var rows =
                db.Query(@"SELECT COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @0", tableName)
                  .ToList();
            if (rows.Any())
            {
                columns.AddRange(rows.Select(row => row[0]).Select(columnName => columnName).Cast<string>());
            }

            return columns;
        }

        public static bool TableExists(this IDatabase db, string tableName)
        {
            var query = db.QuerySingle(@"SELECT * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @0", tableName);
            return query != null;
        }
    }
}