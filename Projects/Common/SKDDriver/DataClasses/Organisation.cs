using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class Organisation
	{
		[Key]
		public Guid UID { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? RemovalDate { get; set; }

		public Guid? ChiefUID { get; set; }
		[ForeignKey("ChiefUID")]
		public Employee Chief { get; set; }

		public Guid? HRChiefUID { get; set; }
		[ForeignKey("HRChiefUID")]
		public Employee HRChief { get; set; }

		public ICollection<OrganisationDoor> OrganisationDoors { get; set; }

		public ICollection<OrganisationUser> OrganisationUsers { get; set; }

		public ICollection<NightSetting> NightSettings { get; set; }

		public ICollection<Employee> Employees { get; set; }

		public ICollection<AdditionalColumnType> AdditionalColumnTypes { get; set; }

		public ICollection<DayInterval> DayIntervals { get; set; }

		public ICollection<Schedule> Schedules { get; set; }

		public ICollection<ScheduleScheme> ScheduleSchemes { get; set; }

		public ICollection<Position> Positions { get; set; }

		public ICollection<Department> Departments { get; set; }

		public ICollection<Holiday> Holidays { get; set; }

		public ICollection<AccessTemplate> AccessTemplates { get; set; }

		public ICollection<PassCardTemplate> PassCardTemplates { get; set; }

		public ICollection<TimeTrackDocumnetType> TimeTrackDocumnetTypes { get; set; }

		public Guid? PhotoUID { get; set; }

		public string Phone { get; set; }

		public string ExternalKey { get; set; }
	}
}
