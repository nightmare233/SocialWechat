IF OBJECT_ID('[t_Social_ConversationField]') IS NULL
BEGIN
	CREATE TABLE [t_Social_ConversationField](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[IfSystem] [bit] NOT NULL,
		[DataType] [smallint] NOT NULL,
		[Name] [nvarchar](256) NOT NULL,
		[SiteId] [int] NOT NULL,
	 CONSTRAINT [PK_t_Social_ConversationField] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] 
END