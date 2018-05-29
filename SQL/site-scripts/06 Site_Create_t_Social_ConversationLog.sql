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
		IF NOT EXISTS (SELECT name FROM sysobjects WHERE type=''U'' AND name=''t_Social_ConversationLog'+@siteIdStr+''')
		AND EXISTS (SELECT 1 FROM [Comm100.General].[dbo].[t_Site] WHERE Id='+@siteIdStr+')
		BEGIN
			CREATE TABLE [t_Social_ConversationLog'+@siteIdStr+'](
				[Id] [int] IDENTITY(1,1) NOT NULL,
				[ConversationId] [int] NOT NULL,
				[Type] [smallint] NOT NULL,
				[Content] [nvarchar](512) NOT NULL,
				[CreatedTime] [datetime] NOT NULL,
				[CreatedBy] [int] NOT NULL
			 CONSTRAINT [PK_t_Social_ConversationLog'+@siteIdStr+'] PRIMARY KEY CLUSTERED 
			(
				[Id] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			) ON [PRIMARY]
			ALTER TABLE [t_Social_ConversationLog'+@siteIdStr+']  WITH CHECK ADD  CONSTRAINT [FK_t_Social_ConversationLog'+@siteIdStr+'_t_Social_Conversation'+@siteIdStr+'_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [t_Social_Conversation'+@siteIdStr+'] ([Id])
			ALTER TABLE [t_Social_ConversationLog'+@siteIdStr+'] CHECK CONSTRAINT [FK_t_Social_ConversationLog'+@siteIdStr+'_t_Social_Conversation'+@siteIdStr+'_ConversationId]
			CREATE INDEX IX_t_Social_ConversationLog'+@siteIdStr+'_ConversationId ON t_Social_ConversationLog'+@siteIdStr+'([ConversationId] ASC)
		END
		'
	EXEC(@sql)
	SET @siteId=@siteId+1
	END
END