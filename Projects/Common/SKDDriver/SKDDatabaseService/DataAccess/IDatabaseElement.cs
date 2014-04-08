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

	public interface IIsDeletedDatabaseElement : IDatabaseElement
	{
		bool IsDeleted { get; set; }
		DateTime RemovalDate { get; set; }
	}

	public interface IOrganizationDatabaseElement : IIsDeletedDatabaseElement
	{
		Guid? OrganizationUID { get; set; }
	}


	
	public partial class Journal : IDatabaseElement { }
	
	public partial class AdditionalColumnType : IOrganizationDatabaseElement { }
	public partial class Department : IOrganizationDatabaseElement { }
	public partial class Document : IOrganizationDatabaseElement { }
	public partial class Employee : IOrganizationDatabaseElement { }
	public partial class EmployeeReplacement : IOrganizationDatabaseElement, ILinkedToEmployee { }
	public partial class Holiday : IOrganizationDatabaseElement { }
	public partial class NamedInterval : IOrganizationDatabaseElement { }
	public partial class Phone : IOrganizationDatabaseElement { }
	public partial class Position : IOrganizationDatabaseElement { }
	public partial class Schedule : IOrganizationDatabaseElement { }
	public partial class ScheduleScheme : IOrganizationDatabaseElement { }
	public partial class AccessTemplate : IOrganizationDatabaseElement { }

	public partial class Day : IIsDeletedDatabaseElement { }
	public partial class ScheduleZoneLink : IIsDeletedDatabaseElement { }
	public partial class Organization : IIsDeletedDatabaseElement { }
	public partial class Interval : IIsDeletedDatabaseElement { }
	public partial class CardZoneLink : IIsDeletedDatabaseElement { }
	public partial class Card : IIsDeletedDatabaseElement, ILinkedToEmployee { }
	public partial class AdditionalColumn : IIsDeletedDatabaseElement, ILinkedToEmployee { }
	public partial class Photo : IIsDeletedDatabaseElement { }
}