---
layout: page
title: Welcome
tagline: about the library
---
{% include JB/setup %}

The BetterMembership.Net library bridges the gap between the [ExtendedMembershipProvider][2] interface which is used with the [WebSecurity][5] API via [SimpleMembershipProvider][4] (see ASP.NET MVC4 Web Application - Internet template), and the older [MembershipProvider][3] interface (see ASP.NET Web Forms template).

By using the BetterMembership.Net library you can take advantage of the newer features of ExtendedMembershipProvider, but still use pre-existing user management tools for out-of-box user administration (including [ASP.NET Web Site Administration Tool][1]). This is not possible when using the vanilla SimpleMembershipProvider implementation of ExtendedMembershipProvider.

The library also overcomes limitations of the SimpleMembershipProvider implementation, and allows multiple provider instances to be configured and used at runtime. This is useful for complex applications that involve multiple authentication schemes for partitioning users, such as in Sitecore for example.

For more information on why SimpleMembershipProvider cannot natively be used as a MembershipProvider, see [SimpleMembershipProvider vs MembershipProvider][1])

#### Features

* Supports SQL Server, SQL Compact and SQL Azure
* Compatible with EntityFramework and code-first
* Uses a clean database schema 
* Works with existing user tables
* Plugs into existing user administration tools (including WSAT)
* Supports initialization of multiple provider instances
* Supports external authentication providers
* Provides customizable validation of username/email/password

#### BetterProfileProvider

When using ExtendedMembershipProvider and the newer WebSecurity interface it's preferable to manage user profile properties via EntityFramework. 

The BetterProfileProvider is provided for backwards compatibility with legacy systems, and can be used side-by-side with EntityFramework and custom database schema. The implementation maps profile properties configured in the web.config to the corresponding columns in the configured user database table.

#### BetterRoleProvider

Like the BetterMembershipProvider, BetterRoleProvider overcomes a limitation of SimpleRoleProvider, and allows multiple instances to be configured and used at runtime. It uses the SimpleRoleProvider database schema, and changes the behaviour of GetRolesForUser to avoid InvalidOperationException when a user is not found. This makes it more compatible with some role management systems that use multiple providers at runtime.

### Installation

	PM> Install-Package BetterMembership.Net
	
[1]: http://www.thecodeking.co.uk/2013/08/simplemembershipprovider-vs.html
[2]: http://msdn.microsoft.com/en-us/library/webmatrix.webdata.extendedmembershipprovider(v=vs.111).aspx
[3]: http://msdn.microsoft.com/en-us/library/system.web.security.membershipprovider.aspx
[4]: http://msdn.microsoft.com/en-us/library/webmatrix.webdata.simplemembershipprovider(v=vs.111).aspx
[5]: http://msdn.microsoft.com/en-us/library/webmatrix.webdata.websecurity(v=vs.111).aspx