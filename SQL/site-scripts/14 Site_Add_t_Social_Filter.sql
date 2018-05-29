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
		VALUES
		(''My Open'', 1, 1,1,0,getdate(),'+@siteIdStr+' ),
		(''My Departments'''' Open'', 2, 1,1,0,getdate(),'+@siteIdStr+' ),
		(''My Offline Department Members'''' Open'', 3, 1,1,0,getdate(),'+@siteIdStr+' ),
		(''All Open'', 4, 1,1,0,getdate(),'+@siteIdStr+' ),
		(''Unassigned'', 5, 1,1,0,getdate(),'+@siteIdStr+' ),
		(''All'', 6, 1,1,0,getdate(),'+@siteIdStr+' )
		'
	EXEC(@sql)
	SET @siteId=@siteId+1
	END
END