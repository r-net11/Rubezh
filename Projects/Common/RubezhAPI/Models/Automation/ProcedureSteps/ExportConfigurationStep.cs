using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ExportConfigurationStep : ProcedureStep
	{
		public ExportConfigurationStep()
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

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ExportConfiguration; } }
	}
}
