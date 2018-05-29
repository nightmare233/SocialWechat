IF OBJECT_ID('[t_Social_TwitterAuth]') IS NULL
BEGIN
CREATE TABLE [t_Social_TwitterAuth](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuthorizationId] [nvarchar](64) NOT NULL,
	[AuthorizationKey] [nvarchar](256) NOT NULL,
	[AuthorizationSecret] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END