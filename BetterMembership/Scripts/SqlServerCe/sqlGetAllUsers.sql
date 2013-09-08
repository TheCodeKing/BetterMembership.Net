Select	total.total Total,
		p.[userId] UserId, 
		[userName] UserName,
		m.[IsConfirmed] IsConfirmed,
		m.[lastPasswordFailureDate] LastPasswordFailureDate,
		m.[PasswordFailuresSinceLastSuccess] PasswordFailuresSinceLastSuccess,
		m.[createDate] creationDate,
		m.[passwordChangedDate] PasswordChangedDate,
		p.[email] Email
From	[UserProfile] p
		inner join [webpages_Membership] m on m.UserId=p.[UserId]
		cross join (
			Select	Count(*) total 
			From	[UserProfile] p1
					inner join [webpages_Membership] m1 on m1.UserId=p1.[UserId]
		) as total
Order By p.[UserId]
OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY