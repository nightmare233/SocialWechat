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
		IF NOT EXISTS (SELECT name FROM sysobjects WHERE type=''U'' AND name=''t_Social_MessageAttachment'+@siteIdStr+''')
		AND EXISTS (SELECT 1 FROM [Comm100.General].[dbo].[t_Site] WHERE Id='+@siteIdStr+')
		BEGIN
			CREATE TABLE [t_Social_MessageAttachment'+@siteIdStr+'](
				[Id] [int] IDENTITY(1,1) NOT NULL,
				[MessageId] [int] NOT NULL,
				[OriginalId] [nvarchar](256) NULL DEFAULT(''''),
				[OriginalLink] [nvarchar](512) NULL DEFAULT(''''),
				[Type] [smallint] NOT NULL,
				[Name] [nvarchar](256) NULL DEFAULT(''''),
				[MimeType] [nvarchar](128) NULL DEFAULT(''''),
				[Size] [bigint] NOT NULL,
				[Url] [nvarchar](512) NULL DEFAULT(''''),
				[PreviewUrl] [nvarchar](512) NULL DEFAULT(''''),
				[RawData] [image] NULL,
			 CONSTRAINT [PK_t_Social_MessageAttachment'+@siteIdStr+'] PRIMARY KEY CLUSTERED 
			(
				[Id] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			) ON [PRIMARY]

			ALTER TABLE [t_Social_MessageAttachment'+@siteIdStr+']  WITH CHECK ADD  CONSTRAINT [FK_t_Social_MessageAttachment'+@siteIdStr+'_t_Social_Message'+@siteIdStr+'_MessageId] FOREIGN KEY([MessageId])
REFERENCES [t_Social_Message'+@siteIdStr+'] ([Id])
			ALTER TABLE [t_Social_MessageAttachment'+@siteIdStr+'] CHECK CONSTRAINT [FK_t_Social_MessageAttachment'+@siteIdStr+'_t_Social_Message'+@siteIdStr+'_MessageId]
			CREATE INDEX IX_t_Social_MessageAttachment'+@siteIdStr+'_MessageId ON t_Social_MessageAttachment'+@siteIdStr+'([MessageId] ASC)
		END
		'
	EXEC(@sql)
	SET @siteId=@siteId+1
	END
END