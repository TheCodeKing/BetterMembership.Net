namespace BetterMembership.Data
{
    using System;
    using System.Collections.Generic;

    internal interface IDatabase : IDisposable
    {
        int Execute(string commandText, params object[] args);

        IEnumerable<dynamic> Query(string commandText, params object[] parameters);

        dynamic QuerySingle(string commandText, params object[] args);

        dynamic QueryValue(string commandText, params object[] parameters);
    }
}