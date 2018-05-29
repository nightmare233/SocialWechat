IF OBJECT_ID('[t_Site_SocialAccount]') IS NULL
BEGIN
	CREATE TABLE [t_Site_SocialAccount](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[SiteId] [int] NOT NULL,
		[FacebookPageId] [nvarchar](128) NULL DEFAULT(''),
		[TwitterUserId] [nvarchar](128) NULL DEFAULT(''),
	 CONSTRAINT [PK_t_Site_SocialAccount] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END