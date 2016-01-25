INSERT INTO Rubezh.dbo.Journals
	(UID, 
	SystemDate, 
	DeviceDate, 
	Subsystem, 
	Name, 
	Description, 
	DescriptionText, 
	ObjectType, 
	ObjectUID, 
	Detalisation, 
	UserName, 
	VideoUID, 
	CameraUID, 
	ObjectName, 
	CardNo) 
SELECT 
	UID, 
	SystemDate, 
	DeviceDate, 
	Subsystem, 
	Name, 
	Description, 
	DescriptionText, 
	1, 
	ObjectUID, 
	Detalisation, 
	UserName, 
	VideoUID, 
	CameraUID, 
	ObjectName, 
	0
FROM Journal_1.dbo.Journal
	