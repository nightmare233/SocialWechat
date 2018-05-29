IF OBJECT_ID('t_Social_ConversationField_Config') IS NOT NULL
AND NOT EXISTS (SELECT * FROM [t_Social_ConversationField_Config])
BEGIN
	INSERT INTO  [t_Social_ConversationField_Config]
	(
		[IfSystem]
		,[DataType]
		,[Name]
	)
	VALUES
	(1,4,'Source'),
	(1,4,'Agent Assignee'),
	(1,4,'Department Assignee'),
	(1,4,'Status'),
	(1,4,'Priority'),
	(1,4,'Social Page/Account'),
	(1,4,'Replied Agent'),
	(1,4,'Last Replied Agent'),
	(1,1,'Last Message Sent by'),
	(1,4,'Agent Assignee Status'),
	(1,4,'Department Assignee Status'),
	(1,1,'Social User'),
	(1,1,'Comment/Messages'),
	(1,3,'Last Message Sent'),
	(1,3,'Date Created'),
	(1,3,'Last Modified Date'),
	(1,2,'Time Since Last Message'),
	(1,2,'Total Messages'),
	(1,2,'Conversation ID')
END
