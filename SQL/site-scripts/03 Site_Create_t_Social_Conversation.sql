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
		IF NOT EXISTS (SELECT name FROM sysobjects WHERE type=''U'' AND name=''t_Social_Conversation'+@siteIdStr+''')
		AND EXISTS (SELECT 1 FROM [Comm100.General].[dbo].[t_Site] WHERE Id='+@siteIdStr+')
		BEGIN
			CREATE TABLE [t_Social_Conversation'+@siteIdStr+'](
				[Id] [int] IDENTITY(1,1) NOT NULL,
				[Source] [smallint] NOT NULL,
				[OriginalId] [nvarchar](256) NULL DEFAULT(''''),
				[IfRead] [bit] DEFAULT(0) NOT NULL,
				[LastMessageSentTime] [datetime] NOT NULL,
				[LastMessageSenderId] [int] NOT NULL,
				[LastRepliedAgentId] [int] NULL,
				[AgentId] [int] NULL,
				[DepartmentId] [int] NULL,
				[Status] [smallint] NOT NULL,
				[Subject] [nvarchar](256) NOT NULL,
				[Priority] [smallint] NOT NULL,
				[Note] [nvarchar](2048) NULL DEFAULT(''''),
				[IsDeleted] [bit] DEFAULT(0) NOT NULL,
				[IsHidden] [bit] DEFAULT(0) NOT NULL,
				[CreatedTime] [datetime] DEFAULT(getdate()) NOT NULL,
				[ModifiedTime] [datetime] NULL DEFAULT(getdate()),
			 CONSTRAINT [PK_t_Social_Conversation'+@siteIdStr+'] PRIMARY KEY CLUSTERED 
			(
				[Id] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			) ON [PRIMARY]
			CREATE INDEX IX_t_Social_Conversation'+@siteIdStr+'_LastMessageSentTime ON t_Social_Conversation'+@siteIdStr+'([LastMessageSentTime] DESC)
			CREATE INDEX IX_t_Social_Conversation'+@siteIdStr+'_LastMessageSenderId ON t_Social_Conversation'+@siteIdStr+'([LastMessageSenderId] ASC)
		END
		'
	EXEC(@sql)
	SET @siteId=@siteId+1
	END
END