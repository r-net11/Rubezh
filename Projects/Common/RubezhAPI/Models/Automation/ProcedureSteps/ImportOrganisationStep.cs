using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public abstract class ImportOrganisationStepBase : ProcedureStep
	{
		public ImportOrganisationStepBase()
		{
			IsWithDeleted = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument IsWithDeleted { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }

		public override Argument[] Arguments
		{
			get { return new Argument[] { IsWithDeleted, PathArgument }; }
		}
	}

	[DataContract]
	public class ImportOrganisationStep : ImportOrganisationStepBase { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ImportOrganisation; } } }

	[DataContract]
	public class ImportOrganisationListStep : ImportOrganisationStepBase { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ImportOrganisationList; } } }

}
