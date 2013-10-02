Select	p.[userName] UserName
From	[UserProfile] p
Where	UPPER(p.[email]) = UPPER(@0)