using System;

namespace SKDDriver.DataAccess
{
	public interface IDatabaseElement
	{
		Guid UID { get; set; }
	}

	public interface ILinkedToEmployee
	{
		Guid? EmployeeUID { get; set; }
	}

	public interface IIsDeletedDatabaseElement
	{
		bool IsDeleted { get; set; }
		DateTime RemovalDate { get; set; }
	}

	public interface IOrganizationDatabaseElement
	{
		Guid? OrganizationUID { get; set; }
	}

	public partial class Journal : IDatabaseElement { }
	public partial class AdditionalColumn : IDatabaseElement, ILinkedToEmployee { }
	public partial class Photo : IDatabaseElement { }

	public partial class Day : IDatabaseElement, IIsDeletedDatabaseElement { }
	public partial class ScheduleZoneLink : IDatabaseElement, IIsDeletedDatabaseElement { }
	public partial class Organization : IDatabaseElement, IIsDeletedDatabaseElement { }
	public partial class Interval : IDatabaseElement, IIsDeletedDatabaseElement { }
	public partial class CardZoneLink : IDatabaseElement, IIsDeletedDatabaseElement { }
	public partial class Card : IDatabaseElement, IIsDeletedDatabaseElement, ILinkedToEmployee { }
	public partial class EmployeeDocument : IDatabaseElement, IIsDeletedDatabaseElement { }
	
	public partial class AdditionalColumnType : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class Department : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class Document : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class Employee : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class EmployeeReplacement : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement, ILinkedToEmployee { }
	public partial class Holiday : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class NamedInterval : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class Phone : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class Position : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class Schedule : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class ScheduleScheme : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }
	public partial class AccessTemplate : IDatabaseElement, IIsDeletedDatabaseElement, IOrganizationDatabaseElement { }

	
}