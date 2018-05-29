IF OBJECT_ID('[t_Social_ConversationFieldOption_Config]') IS NULL
BEGIN
	CREATE TABLE [t_Social_ConversationFieldOption_Config](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[FieldName] [nvarchar](256) NOT NULL,
		[Name] [nvarchar](256) NOT NULL,
		[Value] [nvarchar](256) NOT NULL,
		[Index] [int] NOT NULL,
	 CONSTRAINT [PK_t_Social_ConversationFieldOption_Config] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END