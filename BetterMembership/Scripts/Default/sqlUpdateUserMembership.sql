Update	[webpages_Membership]
Set		IsConfirmed = @1
Where	IsConfirmed != @1 And UserId in (Select p.[userId] From [UserProfile] p Where p.[userId] = @0)
