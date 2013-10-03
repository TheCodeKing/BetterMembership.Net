---
layout: page
title: Usage
tagline: using the library
group: navigation
weight: 1
---
{% include JB/setup %}

The library can be installed via Nuget as follows:

	PM> Install-Package BetterMembership.Net
	
### Configuration

The providers are configured within the web.config in the usual way. Note BetterRoleProvider and BetterProfileProvider require a membershipProviderName attribute. This is used to reference an instance of the BetterMembershipProvider instance from which they will inherit the relevant settings, such as connection string and schema configuration.

	<membership defaultProvider="BetterProvider">
      <providers>
        <add name="BetterProvider" type="BetterMembership.Web.BetterMembershipProvider, BetterMembership, Version=1.0.1.0, Culture=neutral, PublicKeyToken=737bc70442f2c4af" connectionStringName="DefaultConnection" autoInitialize="true" />
      </providers>
    </membership>
    <roleManager defaultProvider="BetterProvider" enabled="true">
      <providers>
        <add name="BetterProvider" type="BetterMembership.Web.BetterRoleProvider, BetterMembership, Version=1.0.1.0, Culture=neutral, PublicKeyToken=737bc70442f2c4af" membershipProviderName="BetterProvider" />
      </providers>
    </roleManager>
    <profile defaultProvider="BetterProvider" enabled="true">
      <providers>
        <add name="BetterProvider" type="BetterMembership.Web.BetterProfileProvider, BetterMembership, Version=1.0.1.0, Culture=neutral, PublicKeyToken=737bc70442f2c4af" membershipProviderName="BetterProvider" />
      </providers>
	  <properties>
	  </properties>
    </profile>
	
### Initialization

The providers can be initialized in one of 2 ways. Either automatically by setting the autoInitialize attribute to true, or alternatively by a call to WebSecurity.InitializeDatabaseConnection once on application startup. Support for multiple providers is only possible when using autoInitialize=true. 

Regardless of how the provider is initialized, the provider must be configured via the web.config with the correct userIdColumn, userNameColumn, and userTableName attributes (see below). When using WebSecurity.InitializeDatabaseConnection for initialization, these values must correspond to the values passed to the API call.

### Customization

The BetterMembershipProvider supports the following properties, which can be defined via attributes set on the BetterMembershipProvider provider config section. The default values are also documented below.

##### AutoInitialize <small>true</small>

Automatically calls WebSecurity.InitializeDatabaseConnection is called once, and forces to run against current provider rather than the default instance.

##### ConnectionStringName <small>DefaultConnection</small>

The database connection to use for database access, specified in calls to WebSecurity.InitializeDatabaseConnection.

##### UserTableName <small>UserProfile</small>

The user table containing user data, specified in calls to WebSecurity.InitializeDatabaseConnection. This may be pre-existing custom user table or a new table. It must contain unique identifier (Int32) and a column that can be used as userName.

##### UserNameColumn <small>UserName</small>

The name of the column to use for storing userName, specified in calls to WebSecurity.InitializeDatabaseConnection.

##### UserIdColumn <small>UserId</small>

The name of the column to use for storing the unique identifier of the user, specified in calls to WebSecurity.InitializeDatabaseConnection.

##### UserEmailColumn <small>Not Set</small>

The name of the column to use for storing the email address of the user, which will then be mapped to MembershipProvider APIs. If not set, then any FindByEmail APIs will treat email as username, and the email property of MembershipUser will be null.

##### AutoCreateTables <small>true</small>

Determines whether to initialize the database schema automatically, specified in calls to WebSecurity.InitializeDatabaseConnection.

##### MaxInvalidPasswordAttempts <small>2147483647</small>

The max number of password attempts during the permitted window before an account is locked out.

##### PasswordAttemptWindow <small>-1</small>

The time in minutes during which consecutive failed password attempts count towards locking out an account. The account is reactivated once the time since the last unsuccessful attempt has lapsed.

##### MinRequiredNonalphanumericCharacters <small>0</small>

The minimum number of non-alphanumeric characters required in user passwords.

##### MinRequiredPasswordLength <small>1</small>

The minimum length for user passwords.

##### RequiresUniqueEmail <small>false</small>

Determines whether a user email address is a required field, and that it must be unique in the database. This cannot be set without also defining UserEmailColumn.

##### MaxEmailLength <small>254</small>

The maximum length for user email addresses.

##### MaxUserNameLength <small>56</small>

The maximum length for usernames.

##### MaxPasswordLength <small>128</small>

The maximum length for user passwords.

##### EmailStrengthRegularExpression <small>^[0-9a-zA-Z.+_-]+@[0-9a-zA-Z.+_-]+\.[a-zA-Z]{2,4}$</small>

The regular expression used to validate user passwords.

##### UserNameRegularExpression <small>^[0-9a-zA-Z_-]+$</small>

The regular expression used to validate usernames.

##### AllowEmailAsUserName <small>true</small>

Enables an email address to be used as the username. This is to provide better out-of-box with external authentication providers without compromising username validation. If set, a username containing @ will be validated as an email address, otherwise it will be validated as username.

##### ApplicationName <small>/</small>

Reports the associated ApplicationName when used in context of  MembershipProvider, the value is otherwise ignored.








