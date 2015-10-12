﻿using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ExportJournalArguments
	{
		public ExportJournalArguments()
		{
			IsExportJournalArgument = new Argument();
			IsExportPassJournalArgument = new Argument();
			MinDateArgument = new Argument();
			MaxDateArgument = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument IsExportJournalArgument { get; set; }

		[DataMember]
		public Argument IsExportPassJournalArgument { get; set; }

		[DataMember]
		public Argument MinDateArgument { get; set; }

		[DataMember]
		public Argument MaxDateArgument { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}

	[DataContract]
	public class ExportOrganisationArguments
	{
		public ExportOrganisationArguments()
		{
			Organisation = new Argument();
			IsWithDeleted = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument Organisation { get; set; }

		[DataMember]
		public Argument IsWithDeleted { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}

	[DataContract]
	public class ExportConfigurationArguments
	{
		public ExportConfigurationArguments()
		{
			IsExportDevices = new Argument();
			IsExportDoors = new Argument();
			IsExportZones = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument IsExportDevices { get; set; }

		[DataMember]
		public Argument IsExportDoors { get; set; }

		[DataMember]
		public Argument IsExportZones { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}

	[DataContract]
	public class ImportOrganisationArguments
	{
		public ImportOrganisationArguments()
		{
			IsWithDeleted = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument IsWithDeleted { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}
}
