﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class Organisation
	{
		public Organisation()
		{
			Doors = new List<OrganisationDoor>();
			Users = new List<OrganisationUser>();
			NightSettings = new List<NightSetting>();
			Employees = new List<Employee>();
			AdditionalColumnTypes = new List<AdditionalColumnType>();
			DayIntervals = new List<DayInterval>();
			Schedules = new List<Schedule>();
			ScheduleSchemes = new List<ScheduleScheme>();
			Positions = new List<Position>();
			Departments = new List<Department>();
			Holidays = new List<Holiday>();
			AccessTemplates = new List<AccessTemplate>();
			PassCardTemplates = new List<PassCardTemplate>();
			TimeTrackDocumnetTypes = new List<TimeTrackDocumnetType>();
		}
		
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

		public Guid? PhotoUID { get; set; }
		public Photo Photo { get; set; }

		public ICollection<OrganisationDoor> Doors { get; set; }

		public ICollection<OrganisationUser> Users { get; set; }

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

		public string Phone { get; set; }

		public string ExternalKey { get; set; }
	}
}
