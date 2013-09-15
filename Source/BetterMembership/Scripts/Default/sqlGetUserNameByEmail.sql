Select	p.[userName] UserName
From	[UserProfile] p
		inner join [webpages_Membership] m on m.UserId=p.[userId]
Where	UPPER(p.[email]) = UPPER(@0)