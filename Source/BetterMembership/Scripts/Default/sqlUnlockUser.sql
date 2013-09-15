Update  m
Set		PasswordFailuresSinceLastSuccess = 0,
		LastPasswordFailureDate = null
From	[UserProfile] p
		inner join webpages_Membership m on m.UserId = p.[userId]
Where	UPPER(p.[userName]) = UPPER(@0)