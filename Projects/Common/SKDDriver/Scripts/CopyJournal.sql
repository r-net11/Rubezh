INSERT INTO Rubezh.dbo.PassJournals
	(UID, 
	EmployeeUID,
	ZoneUID,
	EnterTime,
	ExitTime
	) 
SELECT 
	UID, 
	EmployeeUID,
	ZoneUID,
	EnterTime,
	ExitTime
FROM PassJournal_1.dbo.PassJournal
	