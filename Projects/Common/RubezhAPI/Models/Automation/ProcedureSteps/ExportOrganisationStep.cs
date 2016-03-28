using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public abstract class ExportOrganisationStepBase : ProcedureStep
	{
		public ExportOrganisationStepBase()
		{
			IsWithDeleted = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument IsWithDeleted { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}

	[DataContract]
	public class ExportOrganisationListStep : ExportOrganisationStepBase { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ExportOrganisationList; } } }

	[DataContract]
	public class ExportOrganisationStep : ExportOrganisationStepBase
	{
		public ExportOrganisationStep()
		{
			Organisation = new Argument();
		}

		[DataMember]
		public Argument Organisation { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ExportOrganisation; } }
	}

}
