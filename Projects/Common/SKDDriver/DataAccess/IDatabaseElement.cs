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

		string Name { get; }
	}

	public interface IOrganisationDatabaseElement
	{
		Guid? OrganisationUID { get; set; }
	}

	public interface IExternalKey
	{
		Guid UID { get; set; }

		string ExternalKey { get; set; }

		bool IsDeleted { get; set; }

		DateTime RemovalDate { get; set; }
	}

	public partial class AdditionalColumn : IDatabaseElement, ILinkedToEmployee { }

	public partial class Photo : IDatabaseElement { }

	public partial class NightSetting : IDatabaseElement { }

	public partial class ScheduleZone : IDatabaseElement { }

	public partial class CardDoor : IDatabaseElement { }

	public partial class ScheduleDay : IDatabaseElement { }

	public partial class DayIntervalPart : IDatabaseElement { }

	public partial class Organisation : IDatabaseElement, IIsDeletedDatabaseElement, IExternalKey { }

	public partial class Card : IDatabaseElement, ILinkedToEmployee { }

	public partial class AdditionalColumnType : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement { }

	public partial class Department : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement, IExternalKey { }

	public partial class Holiday : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement { }

	public partial class DayInterval : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement { }

	public partial class Position : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement, IExternalKey { }

	public partial class Schedule : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement { }

	public partial class ScheduleScheme : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement { }

	public partial class AccessTemplate : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement { }

	public partial class PassCardTemplate : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement { }

	public partial class Employee : IDatabaseElement, IIsDeletedDatabaseElement, IOrganisationDatabaseElement, IExternalKey
	{
		public string Name { get { return LastName + " " + FirstName + (SecondName != null ? " " + SecondName : ""); } }
	}

	public partial class Attachment : IDatabaseElement { }

	public partial class Filters : IDatabaseElement { }

	public partial class AccessTemplateDeactivatingReader : IDatabaseElement { }
}