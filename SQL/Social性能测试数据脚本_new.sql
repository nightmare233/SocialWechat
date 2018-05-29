DECLARE @siteId int
SET @siteId=1000007
DECLARE @id nvarchar(100)
DECLARE @index int
DECLARE @senderId int;
DECLARE @receiverId int;
DECLARE @conversationId int;
DECLARE @msgIndex int;
DECLARE @msgType int;
DECLARE @msgPid int;

DECLARE @customer_count int = 100000;
DECLARE @integration_account_count int = 3;
DECLARE @conversation_count int = 400000;
DECLARE @filter_count int = 50;

DECLARE @Ids TABLE (Id INT,IdStr VARCHAR(20)) -- 保存400000个数字序号
DECLARE @L0 TABLE (Num INT)
INSERT INTO @L0 SELECT c FROM (VALUES(1),(1),(1),(1),(1),(1),(1),(1)) AS LO(c)
INSERT INTO @Ids
SELECT TOP (@conversation_count)
ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS RowNum,
CONVERT(nvarchar(100),ROW_NUMBER() OVER (ORDER BY (SELECT NULL))) AS RowNumStr
FROM @L0 AS T1 
CROSS JOIN @L0 AS T2 
CROSS JOIN @L0 AS T3
CROSS JOIN @L0 AS T4
CROSS JOIN @L0 AS T5
CROSS JOIN @L0 AS T6
CROSS JOIN @L0 AS T7
CROSS JOIN @L0 AS T8
ORDER BY RowNum ASC

--/*清空数据*/
--DELETE FROM t_Social_Message1000007;
--DELETE FROM t_Social_Conversation1000007;
--DELETE FROM t_Social_Account WHERE SiteId=@siteId;
--DELETE FROM t_Social_User WHERE SiteId=@siteId;
--DELETE FROM t_Social_Filter;

--/*插入facebook客户数据*/
--INSERT INTO t_Social_User(OriginalId,OriginalLink,Source,Name,Email,Avatar,SiteId,ScreenName,Type,IsDeleted)
--SELECT TOP (@customer_count) 'Fb_Customer_OriginalId_'+ IdStr,'http://www.test.com/fb/'+IdStr,0,'Fb_Customer_'+IdStr,IdStr+'@fb.com','http://www.test.com/'+IdStr,@siteId,'Fb_Customer_ScreenName_'+IdStr,0,0
--FROM @Ids

--/*插入facebook集成账号数据*/
--INSERT INTO t_Social_User(OriginalId,OriginalLink,Source,Name,Email,Avatar,SiteId,ScreenName,Type,IsDeleted)
--SELECT TOP (@integration_account_count) 'Fb_Account_OriginalId_'+ IdStr,'http://www.test.com/fb/'+IdStr,0,'Fb_Account_'+IdStr,IdStr+'@fb.com','http://www.test.com/'+IdStr,@siteId,'Fb_Account_ScreenName_'+IdStr,1,0
--FROM @Ids

--INSERT INTO t_Social_Account(Id,Token,TokenSecret,IfEnable,IfConvertMessageToConversation,
--IfConvertVisitorPostToConversation,IfConvertWallPostToConversation,IfConvertTweetToConversation,FacebookPageCategory,FacebookSignInAs
--,CreatedTime,CreatedBy,ConversationDepartmentId,ConversationAgentId,ConversationPriority,SiteId,IsDeleted)
--SELECT Id,'fb_token',null,1,1,1,1,1,'TEST','TEST',GETDATE(),0,null,null,null,@siteId,0 FROM t_Social_User WHERE Source=0 AND Type=1

--/*插入twitter客户数据*/
--INSERT INTO t_Social_User(OriginalId,OriginalLink,Source,Name,Email,Avatar,SiteId,ScreenName,Type,IsDeleted)
--SELECT TOP (@customer_count) 'Tw_Customer_OriginalId_'+ IdStr,'http://www.test.com/fb/'+IdStr,1,'Tw_Customer_'+IdStr,IdStr+'@tw.com','http://www.test.com/'+IdStr,@siteId,'Tw_Customer_ScreenName_'+IdStr,0,0
--FROM @Ids

--/*插入twitter集成账号数据*/
--INSERT INTO t_Social_User(OriginalId,OriginalLink,Source,Name,Email,Avatar,SiteId,ScreenName,Type,IsDeleted)
--SELECT TOP (@integration_account_count) 'Tw_Account_OriginalId_'+ IdStr,'http://www.test.com/fb/'+IdStr,1,'Tw_Account_'+IdStr,IdStr+'@tw.com','http://www.test.com/'+IdStr,@siteId,'Tw_Account_ScreenName_'+IdStr,1,0
--FROM @Ids

--INSERT INTO t_Social_Account(Id,Token,TokenSecret,IfEnable,IfConvertMessageToConversation,
--IfConvertVisitorPostToConversation,IfConvertWallPostToConversation,IfConvertTweetToConversation,FacebookPageCategory,FacebookSignInAs
--,CreatedTime,CreatedBy,ConversationDepartmentId,ConversationAgentId,ConversationPriority,SiteId,IsDeleted)
--SELECT Id,'tw_token','tw_secret',1,1,1,1,1,'TEST','TEST',GETDATE(),0,null,null,null,@siteId,0 FROM t_Social_User WHERE Source=1 AND Type=1

--DECLARE @minCustomerId INT;
--DECLARE @maxCustomerId INT;
--DECLARE @minAccountId INT;
--DECLARE @maxAccountId INT;

--SELECT @minCustomerId = MIN(Id) FROM t_Social_User WHERE Source=0 AND Type=0
--SELECT @maxCustomerId = MAX(Id) FROM t_Social_User WHERE Source=0 AND Type=0
--SELECT @minAccountId = MIN(Id) FROM t_Social_User WHERE Source=0 AND Type=1
--SELECT @maxAccountId = MAX(Id) FROM t_Social_User WHERE Source=0 AND Type=1


--/*插入facebook message 数据*/

--INSERT INTO t_Social_Conversation1000007 (Source,OriginalId,IfRead,LastMessageSentTime,LastMessageSenderId,Status,Subject,Priority,IsDeleted,IsHidden,CreatedTime,ModifiedTime)
--SELECT 1,'Fb_Message_'+IdStr,0,GETDATE(), @minCustomerId+CEILING(RAND(Id*2)*@customer_count)-1,0,'Fb_Message_Conversation_'+IdStr,1,0,0,GETDATE(),GETDATE() FROM @Ids

--INSERT INTO t_Social_Message1000007(ConversationId,Source,OriginalId,SendTime,SenderId,ReceiverId,Content,IsDeleted)
--SELECT T1.Id,1,'fb message'+T2.IdStr,DATEADD(hour,T2.Id-1,GETDATE()),T1.LastMessageSenderId,@minAccountId+CEILING(RAND(t2.Id*2)*@integration_account_count)-1,'fb message content'+T2.IdStr,0 
--FROM t_Social_Conversation1000007 T1 CROSS JOIN (SELECT TOP 3 * FROM @Ids) T2 WHERE T1.Source=1

--/*插入facebook visitor post 数据*/
--INSERT INTO t_Social_Conversation1000007 (Source,OriginalId,IfRead,LastMessageSentTime,LastMessageSenderId,Status,Subject,Priority,IsDeleted,IsHidden,CreatedTime,ModifiedTime)
--SELECT 2,'Fb_Visit_Post_'+IdStr,0,GETDATE(),@minCustomerId+CEILING(RAND(Id*2)*@customer_count)-1,0,'Fb_Visit_Post_Conversation_'+IdStr,1,0,0,GETDATE(),GETDATE() FROM @Ids;

--INSERT INTO t_Social_Message1000007(ConversationId,Source,OriginalId,ParentId,SendTime,SenderId,ReceiverId,Content,IsDeleted)
--SELECT T1.Id,2,'Fb_Visit_Post'+t2.IdStr,NULL,DATEADD(hour,T2.Id-1,GETDATE()),T1.LastMessageSenderId,@minAccountId+CEILING(RAND(t2.Id*2)*@integration_account_count)-1,
--'Fb Visit Post Content'+T2.IdStr,0
--FROM t_Social_Conversation1000007 T1 CROSS JOIN (SELECT TOP 3 * FROM @Ids) T2 WHERE T1.Source=2

--UPDATE t_Social_Message1000007 SET 
--OriginalId=REPLACE(t1.OriginalId,'Fb_Visit_Post','Fb_Visit_Post_Comment'),
--Content=REPLACE(Content,'Fb Visit Post Content','Fb Visit Post Comment Content'),
--Source=3,
--ParentId =(SELECT t2.Id FROM t_Social_Message1000007 t2 WHERE t2.ConversationId=t1.ConversationId AND t2.OriginalId='Fb_Visit_Post1')
--FROM t_Social_Message1000007 t1
--JOIN t_Social_Conversation1000007 t3 on t1.ConversationId=t3.Id
--WHERE t1.OriginalId!='Fb_Visit_Post1' and t3.Source=2


--/*插入facebook wall post 数据*/
--INSERT INTO t_Social_Conversation1000007 (Source,OriginalId,IfRead,LastMessageSentTime,LastMessageSenderId,Status,Subject,Priority,IsDeleted,IsHidden,CreatedTime,ModifiedTime)
--SELECT 3,'Fb_Wall_Post_'+IdStr,0,GETDATE(),@minCustomerId+CEILING(RAND(Id*2)*@customer_count)-1,0,'Fb_Wall_Post_Conversation_'+IdStr,1,0,0,GETDATE(),GETDATE() FROM @Ids;

--INSERT INTO t_Social_Message1000007(ConversationId,Source,OriginalId,ParentId,SendTime,SenderId,ReceiverId,Content,IsDeleted)
--SELECT T1.Id,2,'Fb_Wall_Post'+t2.IdStr,NULL,DATEADD(hour,T2.Id-1,GETDATE()),T1.LastMessageSenderId,@minAccountId+CEILING(RAND(t2.Id*2)*@integration_account_count)-1,
--'Fb Wall Post Content'+T2.IdStr,0
--FROM t_Social_Conversation1000007 T1 CROSS JOIN (SELECT TOP 3 * FROM @Ids) T2 WHERE T1.Source=3

--UPDATE t_Social_Message1000007 SET 
--OriginalId=REPLACE(t1.OriginalId,'Fb_Wall_Post','Fb_Wall_Post_Comment'),
--Content=REPLACE(Content,'Fb Wall Post Content','Fb Wall Post Comment Content'),
--Source=3,
--ParentId =(SELECT t2.Id FROM t_Social_Message1000007 t2 WHERE t2.ConversationId=t1.ConversationId AND t2.OriginalId='Fb_Wall_Post1')
--FROM t_Social_Message1000007 t1
--JOIN t_Social_Conversation1000007 t3 on t1.ConversationId=t3.Id
--WHERE t1.OriginalId!='Fb_Wall_Post1' and t3.Source=3

--SELECT @minCustomerId = MIN(Id) FROM t_Social_User WHERE Source=1 AND Type=0
--SELECT @maxCustomerId = MAX(Id) FROM t_Social_User WHERE Source=1 AND Type=0
--SELECT @minAccountId = MIN(Id) FROM t_Social_User WHERE Source=1 AND Type=1
--SELECT @maxAccountId = MAX(Id) FROM t_Social_User WHERE Source=1 AND Type=1

--/*插入twitter tweet数据*/
--INSERT INTO t_Social_Conversation1000007 (Source,OriginalId,IfRead,LastMessageSentTime,LastMessageSenderId,Status,Subject,Priority,IsDeleted,IsHidden,CreatedTime,ModifiedTime)
--SELECT 4,'T_Tweet_'+IdStr,0,GETDATE(),@minCustomerId+CEILING(RAND(Id*2)*@customer_count)-1,0,'T_Tweet_Conversation_'+IdStr,1,0,0,GETDATE(),GETDATE() FROM @Ids;

--INSERT INTO t_Social_Message1000007(ConversationId,Source,OriginalId,ParentId,SendTime,SenderId,ReceiverId,Content,IsDeleted)
--SELECT T1.Id,4,'T_Tweet_'+T2.IdStr,NULL,GETDATE(),T1.LastMessageSenderId,@minAccountId+CEILING(RAND(t2.Id*2)*@integration_account_count)-1,'Tweet Content'+T2.IdStr,0
--FROM t_Social_Conversation1000007 T1 CROSS JOIN (SELECT TOP 3 * FROM @Ids) T2 WHERE T1.Source=4

--UPDATE t_Social_Message1000007 SET 
--ParentId =(SELECT t2.Id FROM t_Social_Message1000007 t2 WHERE t2.ConversationId=t1.ConversationId AND t2.OriginalId='T_Tweet_1')
--FROM t_Social_Message1000007 t1
--JOIN t_Social_Conversation1000007 t3 on t1.ConversationId=t3.Id
--WHERE t1.OriginalId!='T_Tweet_1' and t3.Source=4

--/*插入twitter direct message数据*/
--INSERT INTO t_Social_Conversation1000007 (Source,OriginalId,IfRead,LastMessageSentTime,LastMessageSenderId,Status,Subject,Priority,IsDeleted,IsHidden,CreatedTime,ModifiedTime)
--SELECT 5,'T_Direct_Message_'+IdStr,0,GETDATE(),@minCustomerId+CEILING(RAND(Id*2)*@customer_count)-1,0,'T_Direct_Message_Conversation_'+IdStr,1,0,0,GETDATE(),GETDATE() FROM @Ids;

--INSERT INTO t_Social_Message1000007(ConversationId,Source,OriginalId,SendTime,SenderId,ReceiverId,Content,IsDeleted)
--SELECT T1.Id,6,'T_Direct_Message_'+T2.IdStr,DATEADD(hour,T2.Id-1,GETDATE()),LastMessageSenderId,@minAccountId+CEILING(RAND(t2.Id*2)*@integration_account_count)-1,
--'twitter direct message content'+T2.IdStr,0
--FROM t_Social_Conversation1000007 T1 CROSS JOIN (SELECT TOP 3 * FROM @Ids) T2 WHERE T1.Source=5

/*插入filter*/
INSERT INTO t_Social_Filter([Name],[Index],[IfPublic],[Type],[CreatedBy],[CreatedTime],[SiteId])
SELECT TOP (@filter_count) 'Test_Filter_'+IdStr,Id,1,1,0,GETDATE(),@siteId FROM @Ids