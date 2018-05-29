BEGIN
    --DECLARE @siteDbId INT
	DECLARE @siteFrom INT
	DECLARE @siteTo INT

	--SET @siteDbId=21;
	SET @siteFrom=(@siteDbId-1)*500
	SET @siteTo=@siteFrom+500

	DECLARE @siteId INT
	SET @siteId=@siteFrom

	WHILE @siteId < @siteTo
	BEGIN
	    DECLARE @siteIdStr nvarchar(100)
		SET @siteIdStr=CONVERT(nvarchar(100),@siteId)
		DECLARE @sql varchar(max)
		DECLARE @filterId int;

		SET @sql=
		'
		INSERT INTO [t_Social_FilterCondition]
		(
			[FilterId]
			,[FieldId]
			,[MatchType]
			,[Value]
			,[SiteId]
			,[Index]
		)
		VALUES
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''My Open'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Status''),2,''4'','+@siteIdStr+',1),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''My Open'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Agent Assignee''),1,''@Me'','+@siteIdStr+',2),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''My Departments'''' Open'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Status''),2,''4'','+@siteIdStr+',1),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''My Departments'''' Open'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Department Assignee''),1,''@My Department'','+@siteIdStr+',2),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''My Offline Department Members'''' Open'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Status''),2,''4'','+@siteIdStr+',1),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''My Offline Department Members'''' Open'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Agent Assignee''),1,''@My Department Member'','+@siteIdStr+',2),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''My Offline Department Members'''' Open'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Agent Assignee Status''),1,''2'','+@siteIdStr+',3),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''All Open'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Status''),2,''4'','+@siteIdStr+',1),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Unassigned'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Agent Assignee''),1,''Blank'','+@siteIdStr+',1),
		((SELECT [Id] FROM [t_Social_Filter] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Unassigned'' AND [IfPublic] = 1),(SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND Name=''Department Assignee''),1,''Blank'','+@siteIdStr+',2)
		'
	EXEC(@sql)
	SET @siteId=@siteId+1
	END
END