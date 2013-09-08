;with Users as (
	Select	ROW_NUMBER() Over (Order By m.UserId) As Row,
			p.[userId] UserId
	From [UserProfile] p
		inner join [webpages_Membership] m on m.UserId=p.[UserId]
)
Select	(Select COUNT(*) From Users) Total,
		p.[userId] UserId, 
		[userName] UserName,
		m.[IsConfirmed] IsConfirmed,
		m.[lastPasswordFailureDate] LastPasswordFailureDate,
		m.[PasswordFailuresSinceLastSuccess] PasswordFailuresSinceLastSuccess,
		m.[createDate] creationDate,
		m.[passwordChangedDate] PasswordChangedDate,
		p.[email] Email
From	Users u
		inner join [UserProfile] p on u.UserId = p.[UserId]
		inner join [webpages_Membership] m on m.UserId=p.[UserId]
Where	u.Row Between (@0 + 1) AND (@0 + @1)
Order By p.[UserId]