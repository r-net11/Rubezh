using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDDriver.DataAccess
{
	public interface IDatabaseElement
	{
		Guid Uid { get; set; }
		bool IsDeleted { get; set; }
		DateTime RemovalDate { get; set; }
	}

	public interface IOrganizationDatabaseElement : IDatabaseElement
	{
		Guid? OrganizationUid { get; set; }
	}

	public partial class AdditionalColumn : IOrganizationDatabaseElement { }
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

	public partial class ScheduleZoneLink : IDatabaseElement { }
	public partial class Organization : IDatabaseElement { }
	public partial class Journal : IDatabaseElement { }
	public partial class Interval : IDatabaseElement { }
	public partial class Frame : IDatabaseElement { }
	public partial class CardZoneLink : IDatabaseElement { }
	public partial class Card : IDatabaseElement { }
}