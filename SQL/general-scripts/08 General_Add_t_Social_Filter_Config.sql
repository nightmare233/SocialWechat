IF OBJECT_ID('t_Social_Filter_Config') IS NOT NULL
AND NOT EXISTS (SELECT * FROM [t_Social_Filter_Config])
BEGIN
	INSERT INTO [t_Social_Filter_Config]
	(
		[Name]
		,[Index]
		,[IfPublic]
		,[Type]
		,[CreatedBy]
		,[CreatedTime]
	)
	VALUES
	('My Open', 1, 1,1,0,getdate()),
	('My Departments'' Open', 2, 1,1,0,getdate()),
	('My Offline Department Members'' Open', 3, 1,1,0,getdate()),
	('All Open', 4, 1,1,0,getdate()),
	('Unassigned', 5, 1,1,0,getdate()),
	('All', 6, 1,1,0,getdate())
END