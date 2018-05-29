IF OBJECT_ID('[t_Social_Account]') IS NULL
BEGIN
	CREATE TABLE [t_Social_Account](
		[Id] [int] NOT NULL,
		[Token] [nvarchar](512) NOT NULL,
		[TokenSecret] [nvarchar](512) NULL DEFAULT(''),
		[IfEnable] [bit] DEFAULT(1) NOT NULL,
		[IfConvertMessageToConversation] [bit] NOT NULL,
		[IfConvertVisitorPostToConversation] [bit] NOT NULL,
		[IfConvertWallPostToConversation] [bit] NOT NULL,
		[IfConvertTweetToConversation] [bit] NOT NULL,
		[FacebookPageCategory] [nvarchar](256) NULL DEFAULT(''),
		[FacebookSignInAs] [nvarchar](256) NULL DEFAULT(''),
		[CreatedTime] [datetime] NOT NULL,
		[CreatedBy] [int] NOT NULL,
		[ConversationDepartmentId] [int] NULL,
		[ConversationAgentId] [int] NULL,
		[ConversationPriority] [smallint] NULL DEFAULT(1),
		[SiteId] [int] NOT NULL,
		[IsDeleted] [bit] DEFAULT(0) NOT NULL,
	 CONSTRAINT [PK_t_Social_Account] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] 
END