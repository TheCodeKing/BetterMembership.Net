Select	0 Total, 
		p.[userId] UserId, 
		p.[userName] UserName,
		m.[IsConfirmed] IsConfirmed,
		m.[lastPasswordFailureDate] LastPasswordFailureDate,
		m.[PasswordFailuresSinceLastSuccess] PasswordFailuresSinceLastSuccess,
		m.[createDate] creationDate,
		m.[passwordChangedDate] PasswordChangedDate,
		p.[email] Email
From [UserProfile] p
	  inner join [webpages_Membership] m on m.UserId=p.[UserId]
Where UPPER(p.[userName]) = UPPER(@0)