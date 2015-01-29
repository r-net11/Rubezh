using System.Runtime.Serialization;

namespace FiresecAPI.Automation
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
}
