Select	0 Total,
		p.[userId] UserId, 
		p.[userName] UserName,
		m.[IsConfirmed] IsConfirmed,
		m.[lastPasswordFailureDate] LastPasswordFailureDate,
		m.[PasswordFailuresSinceLastSuccess] PasswordFailuresSinceLastSuccess,
		m.[createDate] creationDate,
		m.[passwordChangedDate] PasswordChangedDate,
		p.[email] Email
From	[UserProfile] p
		left join [webpages_Membership] m on m.UserId=p.[UserId]
Where	p.[userId] = @0