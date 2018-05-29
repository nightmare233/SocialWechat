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
		INSERT INTO  [t_Social_ConversationField]
		(
			[IfSystem]
			,[DataType]
			,[Name]
			,[SiteId]
		)
		VALUES
		(1,4,''Source'','+@siteIdStr+'),
		(1,4,''Agent Assignee'','+@siteIdStr+'),
		(1,4,''Department Assignee'','+@siteIdStr+'),
		(1,4,''Status'','+@siteIdStr+'),
		(1,4,''Priority'','+@siteIdStr+'),
		(1,4,''Social Page/Account'','+@siteIdStr+'),
		(1,4,''Replied Agent'','+@siteIdStr+'),
		(1,4,''Last Replied Agent'','+@siteIdStr+'),
		(1,1,''Last Message Sent by'','+@siteIdStr+'),
		(1,4,''Agent Assignee Status'','+@siteIdStr+'),
		(1,4,''Department Assignee Status'','+@siteIdStr+'),
		(1,1,''Social User'','+@siteIdStr+'),
		(1,1,''Comment/Messages'','+@siteIdStr+'),
		(1,3,''Last Message Sent'','+@siteIdStr+'),
		(1,3,''Date Created'','+@siteIdStr+'),
		(1,3,''Last Modified Date'','+@siteIdStr+'),
		(1,2,''Time Since Last Message'','+@siteIdStr+'),
		(1,2,''Total Messages'','+@siteIdStr+'),
		(1,2,''Conversation ID'','+@siteIdStr+')
		'
	EXEC(@sql)
	SET @siteId=@siteId+1
	END
END