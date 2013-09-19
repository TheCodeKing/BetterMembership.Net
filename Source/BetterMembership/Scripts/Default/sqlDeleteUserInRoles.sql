Delete	webpages_UsersInRoles
Where	UserId in (Select p.[userId] From [UserProfile] p Where p.[userName] = @0)