using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecService.Report
{
	[DataContract()]
	public class CardData
	{
		[DataMember()]public string Type { get; set; }
		[DataMember()]public string Number { get; set; }
		[DataMember()]public string Employee { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Position { get; set; }
		[DataMember()]public DateTime Period { get; set; }
	}

	[DataContract()]
	public class DocumentData
	{
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Employee { get; set; }
		[DataMember()]public DateTime StartDateTime { get; set; }
		[DataMember()]public DateTime EndDateTime { get; set; }
		[DataMember()]public string DocumentType { get; set; }
		[DataMember()]public string DocumentName { get; set; }
		[DataMember()]public string DocumentShortName { get; set; }
		[DataMember()]public int DocumentCode { get; set; }
	}

	[DataContract()]
	public class DepartmentData
	{
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Phone { get; set; }
		[DataMember()]public string Chief { get; set; }
		[DataMember()]public string ParentDepartment { get; set; }
		[DataMember()]public string Description { get; set; }
		[DataMember()]public bool IsArchive { get; set; }
		[DataMember()]public int Level { get; set; }
		[DataMember()]public int Index { get; set; }
		[DataMember()]public string Tag { get; set; }
	}

	[DataContract()]
	public class DeviceData
	{
		[DataMember()]public string Nomination { get; set; }
		[DataMember()]public int Number { get; set; }
	}

	[DataContract()]
	public class DisciplineData
	{
		[DataMember()]public DateTime Date { get; set; }
		[DataMember()]public TimeSpan FirstEnter { get; set; }
		[DataMember()]public TimeSpan LastExit { get; set; }
		[DataMember()]public string Employee { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string IsHoliday { get; set; }
		[DataMember()]public TimeSpan Late { get; set; }
		[DataMember()]public TimeSpan EarlyLeave { get; set; }
		[DataMember()]public TimeSpan Absence { get; set; }
		[DataMember()]public TimeSpan Overtime { get; set; }
		[DataMember()]public string DocumentName { get; set; }
		[DataMember()]public string DocumentNo { get; set; }
		[DataMember()]public DateTime DocumentDate { get; set; }
	}

	[DataContract()]
	public class DoorData
	{
		[DataMember()]public int Number { get; set; }
		[DataMember()]public string Door { get; set; }
		[DataMember()]public string Controller { get; set; }
		[DataMember()]public string IP { get; set; }
		[DataMember()]public string EnterReader { get; set; }
		[DataMember()]public string EnterZone { get; set; }
		[DataMember()]public string ExitReader { get; set; }
		[DataMember()]public string ExitZone { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Comment { get; set; }
		[DataMember()]public string Type { get; set; }
	}

	[DataContract()]
	public class EmployeeAccessData
	{
		[DataMember()]public int No { get; set; }
		[DataMember()]public string Zone { get; set; }
		[DataMember()]public string Type { get; set; }
		[DataMember()]public string Number { get; set; }
		[DataMember()]public string Employee { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Position { get; set; }
		[DataMember()]public string Template { get; set; }
		[DataMember()]public object[] ItemArray { get; set; }
	}

	[DataContract()]
	public class EmployeeReportData
	{
		[DataMember()]public List<EmployeeData> EmployeeData { get; set; }
		[DataMember()]public List<EmployeePassCardData> PassCards { get; set; }
		[DataMember()]public List<EmployeeAdditionalColumnsData> AdditionalColumns { get; set; }
	}

	[DataContract()]
	public class EmployeeData
	{
		[DataMember()]public string FirstName { get; set; }
		[DataMember()]public string SecondName { get; set; }
		[DataMember()]public string LastName { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string PositionOrEscort { get; set; }
		[DataMember()]public string Document { get; set; }
		[DataMember()]public string DocumentNumber { get; set; }
		[DataMember()]public byte[] Photo { get; set; }
		[DataMember()]public Guid UID { get; set; }
		[DataMember()]public string Nationality { get; set; }
		[DataMember()]public string Sex { get; set; }
		[DataMember()]public DateTime BirthDay { get; set; }
		[DataMember()]public string BirthPlace { get; set; }
		[DataMember()]public string DocumentIssuer { get; set; }
		[DataMember()]public DateTime DocumentValidFrom { get; set; }
		[DataMember()]public DateTime DocumentValidTo { get; set; }
		[DataMember()]public int Number { get; set; }
		[DataMember()]public string Phone { get; set; }
		[DataMember()]public string Schedule { get; set; }
		[DataMember()]public bool IsEmployee { get; set; }
		[DataMember()]public string Type { get; set; }
	}

	[DataContract()]
	public class EmployeePassCardData
	{
		[DataMember()]public Guid EmployeeUID { get; set; }
		[DataMember()]public int Number { get; set; }
	}

	[DataContract()]
	public class EmployeeAdditionalColumnsData
	{
		[DataMember()]public Guid EmployeeUID { get; set; }
		[DataMember()]public string Name { get; set; }
		[DataMember()]public string Value { get; set; }
	}

	[DataContract()]
	public class EmployeeDoorsData
	{
		[DataMember()]public string AccessPoint { get; set; }
		[DataMember()]public string ZoneIn { get; set; }
		[DataMember()]public string ZoneOut { get; set; }
		[DataMember()]public string Enter { get; set; }
		[DataMember()]public string Exit { get; set; }
		[DataMember()]public string Type { get; set; }
		[DataMember()]public string Number { get; set; }
		[DataMember()]public string Employee { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Position { get; set; }
		[DataMember()]public int NoDoor { get; set; }
		[DataMember()]public int NoEnterZone { get; set; }
		[DataMember()]public int NoExitZone { get; set; }
	}

	[DataContract()]
	public class EmployeeRootReportData
	{
		[DataMember()]public List<EmployeeRootData> Employees { get; set; }
		[DataMember()]public List<EmployeeAdditionalData> Data { get; set; }
	}

	[DataContract()]
	public class EmployeeRootData
	{
		[DataMember()]public string Name { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Position { get; set; }
		[DataMember()]public Guid UID { get; set; }
		[DataMember()]public string Escort { get; set; }
		[DataMember()]public string Description { get; set; }
	}

	[DataContract()]
	public class EmployeeAdditionalData
	{
		[DataMember()]public Guid EmployeeUID { get; set; }
		[DataMember()]public DateTime DateTime { get; set; }
		[DataMember()]public string Zone { get; set; }
	}

	[DataContract()]
	public class EmployeeZonesData
	{
		[DataMember()]public string Employee { get; set; }
		[DataMember()]public string Zone { get; set; }
		[DataMember()]public DateTime EnterDateTime { get; set; }
		[DataMember()]public DateTime ExitDateTime { get; set; }
		[DataMember()]public TimeSpan Period { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Position { get; set; }
		[DataMember()]public string Escort { get; set; }
	}

	[DataContract()]
	public class EventsData
	{
		[DataMember()]public DateTime SystemDateTime { get; set; }
		[DataMember()]public DateTime DeviceDateTime { get; set; }
		[DataMember()]public string Name { get; set; }
		[DataMember()]public string Description { get; set; }
		[DataMember()]public string Object { get; set; }
		[DataMember()]public string User { get; set; }
		[DataMember()]public string System { get; set; }
	}

	[DataContract()]
	public class PositionsData
	{
		[DataMember()]public string Position { get; set; }
		[DataMember()]public string Description { get; set; }
		[DataMember()]public string Organisation { get; set; }
	}

	[DataContract()]
	public class ReflectionData
	{
		[DataMember()]public string NO { get; set; }
		[DataMember()]public string Object { get; set; }
	}

	[DataContract()]
	public class SchedulesData
	{
		[DataMember()]public string Employee { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Position { get; set; }
		[DataMember()]public string Schedule { get; set; }
		[DataMember()]public string ScheduleType { get; set; }
		[DataMember()]public string BaseSchedule { get; set; }
		[DataMember()]public bool UseHoliday { get; set; }
		[DataMember()]public bool FirstEnterLastExit { get; set; }
		[DataMember()]public TimeSpan Delay { get; set; }
		[DataMember()]public TimeSpan LeaveBefore { get; set; }
	}

	[DataContract()]
	public class WorkingTimeData
	{
		[DataMember()]public string Employee { get; set; }
		[DataMember()]public string Department { get; set; }
		[DataMember()]public string Organisation { get; set; }
		[DataMember()]public string Position { get; set; }
		[DataMember()]public double ScheduleDay { get; set; }
		[DataMember()]public double ScheduleNight { get; set; }
		[DataMember()]public double TotalPresence { get; set; }
		[DataMember()]public double Presence { get; set; }
		[DataMember()]public double Overtime { get; set; }
		[DataMember()]public double Night { get; set; }
		[DataMember()]public double Balance { get; set; }
		[DataMember()]public double DocumentOvertime { get; set; }
		[DataMember()]public double DocumentAbsence { get; set; }
		[DataMember()]public double TotalBalance { get; set; }
	}
}
