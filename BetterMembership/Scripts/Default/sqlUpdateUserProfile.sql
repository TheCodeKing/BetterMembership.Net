Update	[UserProfile]
Set		[email] = @1
Where	[email] != @1 AND UPPER([userName]) = UPPER(@0)