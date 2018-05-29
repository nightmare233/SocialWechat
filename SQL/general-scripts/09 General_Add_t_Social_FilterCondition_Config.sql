IF OBJECT_ID('t_Social_FilterCondition_Config') IS NOT NULL
AND NOT EXISTS (SELECT * FROM [t_Social_FilterCondition_Config])
BEGIN
	INSERT INTO [t_Social_FilterCondition_Config]
	(
		[FilterName]
		,[FieldName]
		,[MatchType]
		,[Value]
		,[Index]
	)
	VALUES
	('My Open','Status',2,'4',1),
	('My Open','Agent Assignee',1,'@Me',2),
	('My Departments'' Open','Status',2,'4',1),
	('My Departments'' Open','Department Assignee',1,'@My Department',2),
	('My Offline Department Members'' Open','Status',2,'4',1),
	('My Offline Department Members'' Open','Agent Assignee',1,'@My Department Member',2),
	('My Offline Department Members'' Open','Agent Assignee Status',1,'2',3),
	('All Open','Status',2,'4',1),
	('Unassigned','Agent Assignee',1,'Blank',1),
	('Unassigned','Department Assignee',1,'Blank',2)
END