using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ExportJournalStep : ProcedureStep
	{
		public ExportJournalStep()
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

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ExportJournal; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { IsExportJournalArgument, IsExportPassJournalArgument, MinDateArgument, MaxDateArgument, PathArgument }; }
		}
	}
}
