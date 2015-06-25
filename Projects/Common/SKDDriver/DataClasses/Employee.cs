﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class Employee : IOrganisationItem
	{
		public Employee()
		{
            Guests = new List<Employee>();
            AdditionalColumns = new List<AdditionalColumn>();
            Cards = new List<Card>();
            TimeTrackDocuments = new List<TimeTrackDocument>();
            TimeTrackExceptions = new List<TimeTrackException>();
            JournalItems = new List<Journal>();
            PassJournalItems = new List<PassJournal>();
            EmployeeDays = new List<EmployeeDay>();
		}
		
		#region IOrganisationItemMembers
		[Key]
		public Guid UID { get; set; }

		[NotMapped]
		public string Name
		{
			get { return string.Format("{0} {1} {2}", FirstName, SecondName, LastName); }
			set { return; }
		}

		public string Description { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? RemovalDate { get; set; }

		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }
		#endregion

        public Guid? PositionUID { get; set; }
        public Position Position { get; set; }

        public Guid? DepartmentUID { get; set; }
        public Department Department { get; set; }

        public Guid? PhotoUID { get; set; }
        public Photo Photo { get; set; }

        public Guid? EscortUID { get; set; }
        public Employee Escort { get; set; }
        public ICollection<Employee> Guests { get; set; }

        public Guid? ScheduleUID { get; set; }
        public Schedule Schedule { get; set; }

        public ICollection<AdditionalColumn> AdditionalColumns { get; set; }

        public ICollection<Card> Cards { get; set; }

        public ICollection<TimeTrackDocument> TimeTrackDocuments { get; set; }

        public ICollection<TimeTrackException> TimeTrackExceptions { get; set; }

        public ICollection<Journal> JournalItems { get; set; }

        public ICollection<PassJournal> PassJournalItems { get; set; }

        public ICollection<EmployeeDay> EmployeeDays { get; set; }

		public string FirstName { get; set; }

		public string SecondName { get; set; }

		public string LastName { get; set; }

		public DateTime ScheduleStartDate { get; set; }

		public int Type { get; set; }

		public string TabelNo { get; set; }

		public DateTime CredentialsStartDate { get; set; }

		public string DocumentNumber { get; set; }

		public DateTime BirthDate { get; set; }

		public string BirthPlace { get; set; }

		public DateTime DocumentGivenDate { get; set; }

		public string DocumentGivenBy { get; set; }

		public DateTime DocumentValidTo { get; set; }

		public int Gender { get; set; }

		public string DocumentDepartmentCode { get; set; }

		public string Citizenship { get; set; }

		public int DocumentType { get; set; }

		public string Phone { get; set; }

		public DateTime LastEmployeeDayUpdate { get; set; }

		public string ExternalKey { get; set; }
	}
}
