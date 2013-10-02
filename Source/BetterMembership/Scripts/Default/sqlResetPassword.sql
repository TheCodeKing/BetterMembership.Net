Update  m
Set		PasswordFailuresSinceLastSuccess = 0,
		LastPasswordFailureDate = null,
		[Password] = @1,
		PasswordChangedDate = GetUTCDate(),
		PasswordSalt = ''
From	[UserProfile] p
		inner join webpages_Membership m on m.UserId = p.[userId]
Where	UPPER(p.[userName]) = UPPER(@0)