namespace BetterMembership.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Linq;
    using System.Web.Profile;
    using System.Web.Security;

    using BetterMembership.Data;
    using BetterMembership.Extensions;
    using BetterMembership.Facades;
    using BetterMembership.Utils;

    using CuttingEdge.Conditions;

    using WebMatrix.Data;

    public sealed class BetterProfileProvider : ProfileProvider
    {
        private const string UserNameKey = "UserName";

        private readonly Func<string, IDatabase> databaseFactory;

        private readonly MembershipProviderCollection membershipProviders;

        private readonly Func<string, string, string, string, string, ISqlQueryBuilder> sqlQueryBuilderFactory;

        private IList<string> databaseColumns;

        private BetterMembershipProvider membershipProvider;

        private ISqlQueryBuilder sqlQueryBuilder;

        public BetterProfileProvider()
            : this(
                s => new DatabaseProxy(Database.Open(s)), 
                (a, b, c, d, e) =>
                new SqlQueryBuilder(
                    new SqlResourceFinder(new ResourceManifestFacade(typeof(BetterMembershipProvider).Assembly)), 
                    a, 
                    b, 
                    c, 
                    d, 
                    e), 
                Membership.Providers)
        {
        }

        internal BetterProfileProvider(
            Func<string, IDatabase> databaseFactory, 
            Func<string, string, string, string, string, ISqlQueryBuilder> sqlQueryBuilderFactory, 
            MembershipProviderCollection membershipProviders)
        {
            Condition.Requires(databaseFactory, "databaseFctory").IsNotNull();
            Condition.Requires(sqlQueryBuilderFactory, "sqlQueryBuilderFactory").IsNotNull();
            Condition.Requires(membershipProviders, "membershipProviders").IsNotNull();

            this.databaseFactory = databaseFactory;
            this.sqlQueryBuilderFactory = sqlQueryBuilderFactory;
            this.membershipProviders = membershipProviders;
        }

        public override string ApplicationName { get; set; }

        public override int DeleteInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotSupportedException();
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            Condition.Requires(profiles, "profiles").IsNotNull();

            int i;
            using (var db = this.ConnectToDatabase())
            {
                DeleteUserInRoles(db, profiles);
                DeleteOAuthMembership(db, profiles);
                DeleteMembership(db, profiles);
                i = profiles.Cast<ProfileBase>()
                            .Sum(profile => db.Execute(this.sqlQueryBuilder.DeleteProfile, profile.UserName));
            }

            return i;
        }

        public override int DeleteProfiles(string[] usernames)
        {
            Condition.Requires(usernames, "usernames").IsNotNull();

            int i;
            using (var db = this.ConnectToDatabase())
            {
                DeleteUserInRoles(db, usernames);
                DeleteOAuthMembership(db, usernames);
                DeleteMembership(db, usernames);
                i = usernames.Sum(userName => db.Execute(this.sqlQueryBuilder.DeleteProfile, userName));
            }

            return i;
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, 
            string usernameToMatch, 
            DateTime userInactiveSinceDate, 
            int pageIndex, 
            int pageSize, 
            out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override ProfileInfoCollection FindProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, 
            string usernameToMatch, 
            int pageIndex, 
            int pageSize, 
            out int totalRecords)
        {
            Condition.Requires(usernameToMatch, "usernameToMatch").IsNotNullOrWhiteSpace();
            Condition.Requires(pageIndex, "pageIndex").IsGreaterOrEqual(0);
            Condition.Requires(pageSize, "pageSize").IsGreaterOrEqual(1);

            if (authenticationOption != ProfileAuthenticationOption.All)
            {
                throw new NotSupportedException("only ProfileAuthenticationOption.All is supported");
            }

            using (var db = this.ConnectToDatabase())
            {
                var rows = db.Query(this.sqlQueryBuilder.FindUsersByName, usernameToMatch).ToList();
                return this.ExtractProfileInfoFromRows(rows, out totalRecords);
            }
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, 
            DateTime userInactiveSinceDate, 
            int pageIndex, 
            int pageSize, 
            out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override ProfileInfoCollection GetAllProfiles(
            ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            Condition.Requires(pageIndex, "pageIndex").IsGreaterOrEqual(0);
            Condition.Requires(pageSize, "pageSize").IsGreaterOrEqual(1);

            if (authenticationOption != ProfileAuthenticationOption.All)
            {
                throw new NotSupportedException("only ProfileAuthenticationOption.All is supported");
            }

            var startRow = GetPagingStartRow(pageIndex, pageSize);

            using (var db = this.ConnectToDatabase())
            {
                var rows = db.Query(this.sqlQueryBuilder.GetAllUsers, startRow, pageSize).ToList();
                return this.ExtractProfileInfoFromRows(rows, out totalRecords);
            }
        }

        public override int GetNumberOfInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotSupportedException();
        }

        public override SettingsPropertyValueCollection GetPropertyValues(
            SettingsContext context, SettingsPropertyCollection collection)
        {
            Condition.Requires(context, "context").IsNotNull();
            Condition.Requires(collection, "collection").IsNotNull();

            var userName = context[UserNameKey] as string;
            var values = new SettingsPropertyValueCollection();
            if (string.IsNullOrWhiteSpace(userName))
            {
                return values;
            }

            this.EnsureSupportedColumns();

            if (this.databaseColumns.Count == 0)
            {
                return values;
            }

            var profile = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            using (var db = this.ConnectToDatabase())
            {
                var row = db.QuerySingle(this.sqlQueryBuilder.GetUserProfile, userName);
                if (row != null)
                {
                    for (var i = 0; i < row.Columns.Count; i++)
                    {
                        var key = row.Columns[i];
                        var value = row[key];
                        profile.Add(key, value);
                    }
                }
            }

            foreach (SettingsProperty property in collection)
            {
                if (profile.ContainsKey(property.Name))
                {
                    values.Add(
                        new SettingsPropertyValue(property)
                            {
                                Deserialized = true, 
                                PropertyValue = profile[property.Name]
                            });
                }
            }

            return values;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            Condition.Requires(name, "name").IsNotNullOrWhiteSpace();
            Condition.Requires(config, "config").IsNotNull();

            var membershipProviderName = config.GetString("membershipProviderName");

            if (!string.IsNullOrWhiteSpace(membershipProviderName))
            {
                this.membershipProvider = this.membershipProviders[membershipProviderName] as BetterMembershipProvider;
            }

            if (this.membershipProvider == null)
            {
                throw new ProviderException("membershipProviderName is required");
            }

            config.Remove("membershipProviderName");

            base.Initialize(name, config);

            var providerName = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings[this.membershipProvider.ConnectionStringName];
            if (connectionString != null)
            {
                providerName = connectionString.ProviderName;
            }

            this.sqlQueryBuilder = this.sqlQueryBuilderFactory(
                providerName, 
                this.membershipProvider.UserTableName, 
                this.membershipProvider.UserIdColumn, 
                this.membershipProvider.UserNameColumn, 
                this.membershipProvider.UserEmailColumn);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            Condition.Requires(context, "context").IsNotNull();
            Condition.Requires(collection, "collection").IsNotNull();

            var userName = context[UserNameKey] as string;
            if (string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            this.EnsureSupportedColumns();

            if (this.databaseColumns.Count == 0)
            {
                return;
            }

            var updateValues =
                collection.Cast<SettingsPropertyValue>()
                          .Where(value => this.databaseColumns.Contains(value.Name))
                          .ToList();
            if (updateValues.Count == 0)
            {
                return;
            }

            using (var db = this.ConnectToDatabase())
            {
                object[] values;
                db.Execute(this.sqlQueryBuilder.UpdateUserProfile(updateValues, userName, out values), values);
            }
        }

        private static int GetPagingStartRow(int pageIndex, int pageSize)
        {
            return pageIndex * pageSize;
        }

        private static int GetTotalRecords(List<dynamic> rows)
        {
            return (int)rows[0][0];
        }

        private IDatabase ConnectToDatabase()
        {
            return this.databaseFactory(this.membershipProvider.ConnectionStringName);
        }

        private ProfileInfo CreateProfileInfo(dynamic row)
        {
            string name = row[2];

            return new ProfileInfo(name, true, DateTime.MinValue, DateTime.MinValue, 0);
        }

        private void DeleteMembership(IDatabase db, IEnumerable<string> userNames)
        {
            foreach (var username in userNames)
            {
                db.Execute(this.sqlQueryBuilder.DeleteMembershipUser, username);
            }
        }

        private void DeleteMembership(IDatabase db, ProfileInfoCollection profiles)
        {
            this.DeleteMembership(db, profiles.Cast<ProfileInfo>().Select(x => x.UserName).ToArray());
        }

        private void DeleteOAuthMembership(IDatabase db, IEnumerable<string> userNames)
        {
            foreach (var username in userNames)
            {
                db.Execute(this.sqlQueryBuilder.DeleteOAuthMembershipUser, username);
            }
        }

        private void DeleteOAuthMembership(IDatabase db, ProfileInfoCollection profiles)
        {
            this.DeleteOAuthMembership(db, profiles.Cast<ProfileInfo>().Select(x => x.UserName).ToArray());
        }

        private void DeleteUserInRoles(IDatabase db, IEnumerable<string> userNames)
        {
            foreach (var username in userNames)
            {
                db.Execute(this.sqlQueryBuilder.DeleteUserInRoles, username);
            }
        }

        private void DeleteUserInRoles(IDatabase db, ProfileInfoCollection profiles)
        {
            this.DeleteUserInRoles(db, profiles.Cast<ProfileInfo>().Select(x => x.UserName).ToArray());
        }

        private void EnsureSupportedColumns()
        {
            if (this.databaseColumns != null)
            {
                return;
            }

            this.databaseColumns = new List<string>();
            using (var db = this.ConnectToDatabase())
            {
                var rows =
                    db.Query(
                        @"SELECT COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @0", 
                        this.membershipProvider.UserTableName).ToList();
                if (rows.Any())
                {
                    foreach (var row in rows)
                    {
                        string columnName = row[0];
                        if (string.Compare(
                            columnName, this.membershipProvider.UserIdColumn, StringComparison.OrdinalIgnoreCase) != 0
                            && string.Compare(
                                columnName, this.membershipProvider.UserNameColumn, StringComparison.OrdinalIgnoreCase)
                            != 0)
                        {
                            this.databaseColumns.Add(columnName);
                        }
                    }
                }
            }
        }

        private ProfileInfoCollection ExtractProfileInfoFromRows(List<dynamic> rows, out int totalRecords)
        {
            var profiles = new ProfileInfoCollection();
            totalRecords = 0;
            if (rows.Any())
            {
                totalRecords = GetTotalRecords(rows);
                rows.ForEach(row => profiles.Add(this.CreateProfileInfo(row)));
            }

            return profiles;
        }
    }
}