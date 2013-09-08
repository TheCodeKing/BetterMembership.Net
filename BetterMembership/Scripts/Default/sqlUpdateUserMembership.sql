Update	[webpages_Membership]
Set		IsConfirmed = @1
Where	IsConfirmed != @1 And UserId in (Select p.[UserId] From [UserProfile] p Where UPPER(p.[userName]) = UPPER(@0))
