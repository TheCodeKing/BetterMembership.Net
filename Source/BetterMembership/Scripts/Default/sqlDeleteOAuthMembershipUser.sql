Delete	webpages_OAuthMembership
Where	UserId in (Select p.[userId] From [UserProfile] p Where p.[userName] = @0)