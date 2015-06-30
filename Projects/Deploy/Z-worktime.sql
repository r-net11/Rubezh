DROP PROCEDURE Z_WORKTIME
GO

EXEC master.dbo.sp_dropserver @server=N'SKD_LINKSERVER', @droplogins='droplogins'
GO

EXEC master.dbo.sp_addlinkedserver @server = N'SKD_LINKSERVER', @srvproduct=N'SQLSERVER', @provider=N'SQLNCLI', @datasrc=N'.\SQLEXPRESS'
GO
CREATE PROCEDURE Z_WORKTIME
	@DT nvarchar(16)
AS
BEGIN
	DECLARE @Year AS INT
	DECLARE @Month AS INT
	DECLARE @Day AS INT

	DECLARE @DTLength AS INT
	SET @DTLength = DATALENGTH(@DT)

	IF(@DTLength = 8)
	BEGIN
		SET @Year = CAST(SUBSTRING(@DT, 1, 4) AS INT)
		SET @Month = 0
		SET @Day = 0
	END
	IF(@DTLength = 12)
	BEGIN
		SET @Year = CAST(SUBSTRING(@DT, 1, 4) AS INT)
		SET @Month = CAST(SUBSTRING(@DT, 5, 2) AS INT)
	SET @Day = 0
	END
	IF(@DTLength = 16)
	BEGIN
		SET @Year = CAST(SUBSTRING(@DT, 1, 4) AS INT)
		SET @Month = CAST(SUBSTRING(@DT, 5, 2) AS INT)
		SET @Day = CAST(SUBSTRING(@DT, 7, 2) AS INT)
	END

	SELECT
		CONVERT(VARCHAR(10),p.EnterTime,120) AS 'DCREATE',
		CONVERT(VARCHAR(8),p.EnterTime,108) AS 'TCREATE',
		'1' AS 'IOFLAG',
		e.TabelNo AS 'KLUCH',
		e.LastName + ' ' + e.FirstName + ' ' + e.SecondName AS 'FIO',
		p.ZoneUID AS 'NAMEROOM'
	FROM PassJournal p
	JOIN SKD_LINKSERVER.SKD.dbo.Employee e ON p.EmployeeUID = e.UID
	WHERE YEAR(EnterTime) = @Year AND (@Month = 0 OR Month(EnterTime) = @Month) AND (@Day = 0 OR DAY(EnterTime) = @Day)

	UNION

	SELECT
		CONVERT(VARCHAR(10),p.ExitTime,120) AS 'DCREATE',
		CONVERT(VARCHAR(8),p.ExitTime,108) AS 'TCREATE',
		'0' AS 'IOFLAG',
		e.TabelNo AS 'KLUCH',
		e.LastName + ' ' + e.FirstName + ' ' + e.SecondName AS 'FIO',
		p.ZoneUID AS 'NAMEROOM'
	FROM PassJournal p
	JOIN SKD_LINKSERVER.SKD.dbo.Employee e ON p.EmployeeUID = e.UID
	WHERE YEAR(ExitTime) = @Year AND (@Month = 0 OR Month(ExitTime) = @Month) AND (@Day = 0 OR DAY(ExitTime) = @Day)
END
GO

EXECUTE Z_WORKTIME N'20141109'