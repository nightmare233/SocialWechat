IF OBJECT_ID('t_Social_ConversationFieldOption_Config') IS NOT NULL
AND NOT EXISTS (SELECT * FROM [t_Social_ConversationFieldOption_Config])
BEGIN

	INSERT INTO [t_Social_ConversationFieldOption_Config]
		(
			[FieldName]
			,[Name]
			,[Value]
			,[Index]
		)
	VALUES
		( 'Source','Facebook Message','1',1),
		( 'Source','Facebook Visitor Post','2',2),
		( 'Source','Facebook Wall Post','3',3),
		( 'Source','Twitter Tweet','4',4),
		( 'Source','Twitter Direct Message','5',5),
		( 'Status','New','0',1),
		( 'Status','Pending Internal','1',2),
		( 'Status','Pending External','2',3),
		( 'Status','On Hold','3',4),
		( 'Status','Closed','4',5),
		( 'Priority','Low','0',1),
		( 'Priority','Normal','1',2),
		( 'Priority','High','2',3),
		( 'Priority','Urgent','3',4)
	
END