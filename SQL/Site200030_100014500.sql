SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
CREATE TABLE [t_Social_User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OriginalId] [nvarchar](256) NOT NULL,
	[OriginalLink] [nvarchar](512) NULL DEFAULT(''),
	[Source] [smallint] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Email] [nvarchar](256) NULL DEFAULT(''),
	[Avatar] [nvarchar](256) NULL DEFAULT(''),
	[SiteId] [int] NOT NULL,
	[ScreenName] [nvarchar](256) NULL DEFAULT(''),
	[Type] [smallint] NOT NULL,
	[IsDeleted] [bit] DEFAULT(0) NOT NULL,
 CONSTRAINT [PK_t_Social_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [t_Social_Account]  WITH CHECK ADD  CONSTRAINT [FK_t_Social_Account_t_Social_User_Id] FOREIGN KEY([Id])
REFERENCES [t_Social_User] ([Id])
GO
ALTER TABLE [t_Social_Account] CHECK CONSTRAINT [FK_t_Social_Account_t_Social_User_Id]
GO
CREATE TABLE [t_Social_Conversation100014500](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Source] [smallint] NOT NULL,
	[OriginalId] [nvarchar](256) NULL DEFAULT(''),
	[IfRead] [bit] DEFAULT(0) NOT NULL,
	[LastMessageSentTime] [datetime] NOT NULL,
	[LastMessageSenderId] [int] NOT NULL,
	[LastRepliedAgentId] [int] NULL,
	[AgentId] [int] NULL,
	[DepartmentId] [int] NULL,
	[Status] [smallint] NOT NULL,
	[Subject] [nvarchar](256) NOT NULL,
	[Priority] [smallint] NOT NULL,
	[Note] [nvarchar](2048) NULL DEFAULT(''),
	[IsDeleted] [bit] DEFAULT(0) NOT NULL,
	[IsHidden] [bit] DEFAULT(0) NOT NULL,
	[CreatedTime] [datetime] DEFAULT(getdate()) NOT NULL,
	[ModifiedTime] [datetime] NULL DEFAULT(getdate()),
	CONSTRAINT [PK_t_Social_Conversation100014500] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE INDEX IX_t_Social_Conversation100014500_LastMessageSentTime ON t_Social_Conversation100014500([LastMessageSentTime] DESC)
GO
CREATE INDEX IX_t_Social_Conversation100014500_LastMessageSenderId ON t_Social_Conversation100014500(LastMessageSenderId ASC)
GO
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
GO
CREATE TABLE [t_Social_ConversationFieldOption](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FieldId] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
	[Index] [int] NOT NULL,
	[SiteId] [int] NOT NULL,
 CONSTRAINT [PK_t_Social_ConversationFieldOption] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [t_Social_ConversationFieldOption]  WITH CHECK ADD  CONSTRAINT [FK_t_Social_ConversationFieldOption_t_Social_ConversationField_FieldId] FOREIGN KEY([FieldId])
REFERENCES [t_Social_ConversationField] ([Id])
GO
ALTER TABLE [t_Social_ConversationFieldOption] CHECK CONSTRAINT [FK_t_Social_ConversationFieldOption_t_Social_ConversationField_FieldId]
GO
CREATE TABLE [t_Social_ConversationLog100014500](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConversationId] [int] NOT NULL,
	[Type] [smallint] NOT NULL,
	[Content] [nvarchar](512) NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL
	CONSTRAINT [PK_t_Social_ConversationLog100014500] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [t_Social_ConversationLog100014500]  WITH CHECK ADD  CONSTRAINT [FK_t_Social_ConversationLog100014500_t_Social_Conversation100014500_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [t_Social_Conversation100014500] ([Id])
GO
ALTER TABLE [t_Social_ConversationLog100014500] CHECK CONSTRAINT [FK_t_Social_ConversationLog100014500_t_Social_Conversation100014500_ConversationId]
GO
CREATE INDEX IX_t_Social_ConversationLog100014500_ConversationId ON t_Social_ConversationLog100014500([ConversationId] ASC) 
GO
CREATE TABLE [t_Social_Filter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Index] [int] DEFAULT(0) NOT NULL,
	[IfPublic] [bit] NOT NULL,
	[Type] [smallint] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[SiteId] [int] NOT NULL,
	[LogicalExpression] [nvarchar](256) NULL DEFAULT(''),
 CONSTRAINT [PK_t_Social_Filter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [t_Social_FilterCondition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FilterId] [int] NOT NULL,
	[FieldId] [int] NOT NULL,
	[MatchType] [smallint] NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
	[SiteId] [int] NOT NULL,
	[Index] [int] DEFAULT(0) NOT NULL,
 CONSTRAINT [PK_t_Social_FilterCondition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [t_Social_FilterCondition]  WITH CHECK ADD  CONSTRAINT [FK_t_Social_FilterCondition_t_Social_ConversationField_FieldId] FOREIGN KEY([FieldId])
REFERENCES [t_Social_ConversationField] ([Id])
GO
ALTER TABLE [t_Social_FilterCondition] CHECK CONSTRAINT [FK_t_Social_FilterCondition_t_Social_ConversationField_FieldId]
GO
ALTER TABLE [t_Social_FilterCondition]  WITH CHECK ADD  CONSTRAINT [FK_t_Social_FilterCondition_t_Social_Filter_FilterId] FOREIGN KEY([FilterId])
REFERENCES [t_Social_Filter] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [t_Social_FilterCondition] CHECK CONSTRAINT [FK_t_Social_FilterCondition_t_Social_Filter_FilterId]
GO
CREATE TABLE [t_Social_Message100014500](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConversationId] [int] NOT NULL,
	[Source] [smallint] NOT NULL,
	[OriginalId] [nvarchar](256) NOT NULL,
	[OriginalLink] [nvarchar](512) NULL DEFAULT(''),
	[ParentId] [int] NULL,
	[SendTime] [datetime] NOT NULL,
	[SenderId] [int] NOT NULL,
	[ReceiverId] [int] NULL,
	[Content] [nvarchar](2048) NULL DEFAULT(''),
	[Story] [nvarchar](512) NULL DEFAULT(''),
	[IsDeleted] [bit] DEFAULT(0) NOT NULL,
	[QuoteTweetId] [nvarchar](256) NULL DEFAULT(''),
	[SendAgentId] [int] NULL,
	CONSTRAINT [PK_t_Social_Message100014500] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [t_Social_Message100014500]  WITH CHECK ADD  CONSTRAINT [FK_t_Social_Message100014500_t_Social_Conversation100014500_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [t_Social_Conversation100014500] ([Id])
GO
ALTER TABLE [t_Social_Message100014500] CHECK CONSTRAINT [FK_t_Social_Message100014500_t_Social_Conversation100014500_ConversationId]
GO
CREATE INDEX IX_t_Social_Message100014500_OriginalId_Source ON t_Social_Message100014500([OriginalId],[Source] ASC)
GO
CREATE INDEX IX_t_Social_Message100014500_SenderId ON t_Social_Message100014500([SenderId] ASC)
GO
CREATE INDEX IX_t_Social_Message100014500_ReceiverId ON t_Social_Message100014500([ReceiverId] ASC)
GO
CREATE INDEX IX_t_Social_Message100014500_ConversationId ON t_Social_Message100014500([ConversationId] ASC)
GO
CREATE TABLE [t_Social_MessageAttachment100014500](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NOT NULL,
	[OriginalId] [nvarchar](256) NULL DEFAULT(''),
	[OriginalLink] [nvarchar](512) NULL DEFAULT(''),
	[Type] [smallint] NOT NULL,
	[Name] [nvarchar](256) NULL DEFAULT(''),
	[MimeType] [nvarchar](100) NULL DEFAULT(''),
	[Size] [bigint] NOT NULL,
	[Url] [nvarchar](512) NULL DEFAULT(''),
	[PreviewUrl] [nvarchar](512) NULL DEFAULT(''),
	[RawData] [image] NULL,
	CONSTRAINT [PK_t_Social_MessageAttachment100014500] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [t_Social_MessageAttachment100014500]  WITH CHECK ADD  CONSTRAINT [FK_t_Social_MessageAttachment100014500_t_Social_Message100014500_MessageId] FOREIGN KEY([MessageId])
REFERENCES [t_Social_Message100014500] ([Id])
GO
ALTER TABLE [t_Social_MessageAttachment100014500] CHECK CONSTRAINT [FK_t_Social_MessageAttachment100014500_t_Social_Message100014500_MessageId]
GO
CREATE INDEX IX_t_Social_MessageAttachment100014500_MessageId ON t_Social_MessageAttachment100014500([MessageId] ASC)
GO
CREATE TABLE [t_Social_TwitterAuth](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuthorizationId] [nvarchar](50) NOT NULL,
	[AuthorizationKey] [nvarchar](256) NOT NULL,
	[AuthorizationSecret] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO  [t_Social_ConversationField]
(
	[IfSystem]
	,[DataType]
	,[Name]
	,[SiteId]
)
SELECT
	[IfSystem]
	,[DataType]
	,[Name]
	,100014500 AS [SiteId]
	FROM [Comm100.General].[dbo].[t_Social_ConversationField_Config]
GO
INSERT INTO [t_Social_ConversationFieldOption]
(
	[FieldId]
	,[Name]
	,[Value]
	,[Index]
	,[SiteId]
)
 SELECT
	(SELECT [Id]AS [FieldId] FROM [t_Social_ConversationField] WHERE [SiteId] = 100014500 AND [Name] = p.FieldName )
	, [Name]
	,[Value]
	,[Index]
	,100014500 AS [SiteId]
	FROM [Comm100.General].[dbo].[t_Social_ConversationFieldOption_Config] p
GO
INSERT INTO [t_Social_Filter]
(
	[Name]
	,[Index]
	,[IfPublic]
	,[Type]
	,[CreatedBy]
	,[CreatedTime]
	,[SiteId]
)
SELECT 
	[Name]
	,[Index]
	,[IfPublic]
	,[Type]
	,[CreatedBy]
	,getdate()
	,100014500 AS [SiteId]
	FROM [Comm100.General].[dbo].[t_Social_Filter_Config]
GO
INSERT INTO [t_Social_FilterCondition]
(
	[FilterId]
	,[FieldId]
	,[MatchType]
	,[Value]
	,[SiteId]
	,[Index]
)
SELECT 
	(SELECT [Id] AS [FilterId] FROM [t_Social_Filter] WHERE [SiteId] = 100014500 AND [Name] = p.FilterName)
	,(SELECT [Id] AS [FieldId] FROM [t_Social_ConversationField] WHERE [SiteId] = 100014500 AND [Name] = p.FieldName)
	,[MatchType]
	,[Value]
	,100014500 AS [SiteId]
	,[Index]
	FROM [Comm100.General].[dbo].[t_Social_FilterCondition_Config] p
GO

