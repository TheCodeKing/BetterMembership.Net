---
layout: page
title: Compatibility
tagline: pros & cons
group: navigation
weight: 2
---
{% include JB/setup %}

The BetterMembership.Net providers use the database schema from SimpleMemberhipProviders which are not naturally compatible with the legacy MembershipProvider interfaces, therefore password retrieval via question & answer is not supported, and none of the APIs for querying inactive/online state are supported.

### Extended Implementation

The BetterMembership.Net library extends SimpleMembershipProvider and implements the following unsupported methods. This providers back support for the MembershipProvider interface from which it extends.
	
* CreateUser
* GetUser<sup>+</sup>
* GetUserNameByEmail
* FindUserByUserName
* FindUserByEmail
* GetAllUsers
* FindUsersByName
* FindUsersByEmail
* UnlockUser
* ResetPassword	

<small><sup>+</sup> The userIsOnline flag is ignored, but may be implemented in the future.</small>

#### MembershipUser

The library maps properties on the legacy MembershipUser instance as follows.

* CreationDate -> WebPages_Membership.CreatedDate
* IsApproved -> WebPages_Membership.IsConfirmed
* LastPasswordChangedDate -> WebPages_Membership.PasswordChangedDate
* IsLockedOut -> Based on configured maxInvalidPasswordAttempts/passwordAttemptWindow
* UserProviderKey -> Configured userIdColumn on user table
* Username -> Configured userNameColumn on user table
* Email -> Configured userEmailColumn on user table

### Limitations

The following MembershipProvider methods are not supported due to incompatibilities with the older APIs and database schema.

* GetPassword
* GetNumberOfUsersOnline
* ChangePasswordQuestionAndAnswer
* ResetPassword<sup>+</sup>

<small><sup>+</sup> The method is supported when specifying empty/null answer.</small>

The following MembershipUser properties are not supported and will provide default values of null, false, or DateTime.MinValue.

* Comment
* IsOnline
* LastLoginDate
* LastActivityDate	
* LastLockoutDate
* PaswordQuestion
* PaswordAnswer	

#### ProfileProvider Limitations

The following ProfileProvider methods are not supported, due to incompatibilities with the older APIs and database schema.

* GetAllProfiles<sup>+</sup>
* DeleteInactiveProfiles
* GetAllInactiveProfiles
* GetNumberOfInactiveProfiles
* FindInactiveProfilesByUserName
* FindProfilesByUserName<sup>+</sup>

<small><sup>+</sup> The method is supported when specifying ProfileAuthenticationOption.All.</small>

[1]: http://msdn.microsoft.com/en-us/library/yy40ytx0(v=vs.100).aspx