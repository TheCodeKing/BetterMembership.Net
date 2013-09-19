Delete	webpages_Membership
Where	UserId in (Select p.[userId] From [UserProfile] p Where p.[userName] = @0)