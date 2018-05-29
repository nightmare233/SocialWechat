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
		SET @sql=
		'
		INSERT INTO [t_Social_ConversationFieldOption]
		  (
			   [FieldId]
			  ,[Name]
			  ,[Value]
			  ,[Index]
			  ,[SiteId]
		  )
		VALUES
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Source'' AND [DataType] = 4),''Facebook Message'',''1'',1,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Source'' AND [DataType] = 4),''Facebook Visitor Post'',''2'',2,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Source'' AND [DataType] = 4),''Facebook Wall Post'',''3'',3,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Source'' AND [DataType] = 4),''Twitter Tweet'',''4'',4,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Source'' AND [DataType] = 4),''Twitter Direct Message'',''5'',5,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Status'' AND [DataType] = 4),''New'',''0'',1,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Status'' AND [DataType] = 4),''Pending Internal'',''1'',2,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Status'' AND [DataType] = 4),''Pending External'',''2'',3,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Status'' AND [DataType] = 4),''On Hold'',''3'',4,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Status'' AND [DataType] = 4),''Closed'',''4'',5,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Priority'' AND [DataType] = 4),''Low'',''0'',1,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Priority'' AND [DataType] = 4),''Normal'',''1'',2,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Priority'' AND [DataType] = 4),''High'',''2'',3,'+@siteIdStr+'),
		  ( (SELECT [Id] FROM [t_Social_ConversationField] WHERE [SiteId] = '+@siteIdStr+' AND [Name] = ''Priority'' AND [DataType] = 4),''Urgent'',''3'',4,'+@siteIdStr+')
		'
	EXEC(@sql)
	SET @siteId=@siteId+1
	END
END