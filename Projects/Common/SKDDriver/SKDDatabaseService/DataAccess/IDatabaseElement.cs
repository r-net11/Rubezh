using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDDriver.DataAccess
{
	public interface IDatabaseElement
	{
		Guid UID { get; set; }
	}

	public interface IIsDeletedDatabaseElement : IDatabaseElement
	{
		Guid UID { get; set; }
		bool IsDeleted { get; set; }
		DateTime RemovalDate { get; set; }
	}

	public interface IOrganizationDatabaseElement : IIsDeletedDatabaseElement
	{
		Guid? OrganizationUID { get; set; }
	}
	
	public partial class Journal : IDatabaseElement { }
	
	public partial class AdditionalColumnType : IOrganizationDatabaseElement { }
	public partial class Day : IOrganizationDatabaseElement { }
	public partial class Department : IOrganizationDatabaseElement { }
	public partial class Document : IOrganizationDatabaseElement { }
	public partial class Employee : IOrganizationDatabaseElement { }
	public partial class EmployeeReplacement : IOrganizationDatabaseElement { }
	public partial class Holiday : IOrganizationDatabaseElement { }
	public partial class NamedInterval : IOrganizationDatabaseElement { }
	public partial class Phone : IOrganizationDatabaseElement { }
	public partial class Position : IOrganizationDatabaseElement { }
	public partial class Schedule : IOrganizationDatabaseElement { }
	public partial class ScheduleScheme : IOrganizationDatabaseElement { }
	public partial class GUD : IOrganizationDatabaseElement { }

	public partial class ScheduleZoneLink : IIsDeletedDatabaseElement { }
	public partial class Organization : IIsDeletedDatabaseElement { }
	public partial class Interval : IIsDeletedDatabaseElement { }
	public partial class CardZoneLink : IIsDeletedDatabaseElement { }
	public partial class Card : IIsDeletedDatabaseElement { }
	public partial class AdditionalColumn : IIsDeletedDatabaseElement { }
	public partial class Photo : IIsDeletedDatabaseElement { }
}