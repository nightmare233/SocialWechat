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
		IF NOT EXISTS (SELECT name FROM sysobjects WHERE type=''U'' AND name=''t_Social_Message'+@siteIdStr+''')
		AND EXISTS (SELECT 1 FROM [Comm100.General].[dbo].[t_Site] WHERE Id='+@siteIdStr+')
		BEGIN
			CREATE TABLE [t_Social_Message'+@siteIdStr+'](
				[Id] [int] IDENTITY(1,1) NOT NULL,
				[ConversationId] [int] NOT NULL,
				[Source] [smallint] NOT NULL,
				[OriginalId] [nvarchar](256) NOT NULL,
				[OriginalLink] [nvarchar](512) NULL DEFAULT(''''),
				[ParentId] [int] NULL,
				[SendTime] [datetime] NOT NULL,
				[SenderId] [int] NOT NULL,
				[ReceiverId] [int] NULL,
				[Content] [nvarchar](2048) NULL DEFAULT(''''),
				[Story] [nvarchar](512) NULL DEFAULT(''''),
				[IsDeleted] [bit] DEFAULT(0) NOT NULL,
				[QuoteTweetId] [nvarchar](256) NULL DEFAULT(''''),
				[SendAgentId] [int] NULL,
			 CONSTRAINT [PK_t_Social_Message'+@siteIdStr+'] PRIMARY KEY CLUSTERED 
			(
				[Id] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			) ON [PRIMARY]
			ALTER TABLE [t_Social_Message'+@siteIdStr+']  WITH CHECK ADD  CONSTRAINT [FK_t_Social_Message'+@siteIdStr+'_t_Social_Conversation'+@siteIdStr+'_ConversationId] FOREIGN KEY([ConversationId])
			REFERENCES [t_Social_Conversation'+@siteIdStr+'] ([Id])
			ALTER TABLE [t_Social_Message'+@siteIdStr+'] CHECK CONSTRAINT [FK_t_Social_Message'+@siteIdStr+'_t_Social_Conversation'+@siteIdStr+'_ConversationId]
			CREATE INDEX IX_t_Social_Message'+@siteIdStr+'_OriginalId_Source ON t_Social_Message'+@siteIdStr+'([OriginalId],[Source] ASC)
			CREATE INDEX IX_t_Social_Message'+@siteIdStr+'_SenderId ON t_Social_Message'+@siteIdStr+'([SenderId] ASC)
	        CREATE INDEX IX_t_Social_Message'+@siteIdStr+'_ReceiverId ON t_Social_Message'+@siteIdStr+'([ReceiverId] ASC)
			CREATE INDEX IX_t_Social_Message'+@siteIdStr+'_ConversationId ON t_Social_Message'+@siteIdStr+'([ConversationId] ASC)
		END
		'
	EXEC(@sql)
	SET @siteId=@siteId+1
	END
END