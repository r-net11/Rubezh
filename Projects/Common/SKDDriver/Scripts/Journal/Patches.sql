USE [Journal_0]
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'VideoUID')
BEGIN
	ALTER TABLE Journal ADD VideoUID uniqueidentifier NULL
	INSERT INTO Patches (Id) VALUES ('VideoUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'CameraUID')
BEGIN
	ALTER TABLE Journal ADD CameraUID uniqueidentifier NULL
	INSERT INTO Patches (Id) VALUES ('CameraUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ErrorCode')
BEGIN
	ALTER TABLE Journal ADD ErrorCode int NULL
	INSERT INTO Patches (Id) VALUES ('ErrorCode')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'ControllerUID')
BEGIN
	ALTER TABLE Journal ADD ControllerUID uniqueidentifier NULL
	INSERT INTO Patches (Id) VALUES ('ControllerUID')
END
GO
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'GetLastJournalItemProducedByController')
BEGIN
	exec [dbo].[sp_executesql] @statement = N'
CREATE PROCEDURE [dbo].[GetLastJournalItemProducedByController]
	@contollerUid uniqueidentifier
AS
BEGIN
	SELECT TOP 1 *
	FROM dbo.Journal
	WHERE [ControllerUID]=@contollerUid
	ORDER BY [DeviceDate] DESC
END'	
	INSERT INTO Patches (Id) VALUES ('GetLastJournalItemProducedByController')
END
GO

-- В таблицу 'Journal' добавлено поле 'EventID'
IF NOT EXISTS (SELECT * FROM Patches WHERE Id = 'Column_EventID_Added_In_Table_Journal')
	BEGIN
		ALTER TABLE [dbo].[Journal] ADD [EventID] [int] NULL
		INSERT INTO [dbo].[Patches] (Id) VALUES ('Column_EventID_Added_In_Table_Journal')
	END
GO
