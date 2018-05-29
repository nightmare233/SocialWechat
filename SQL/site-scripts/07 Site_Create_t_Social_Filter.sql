IF OBJECT_ID('[t_Social_Filter]') IS NULL
BEGIN
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
END