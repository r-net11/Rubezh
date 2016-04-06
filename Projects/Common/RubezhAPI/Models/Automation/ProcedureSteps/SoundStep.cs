using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class SoundStep : UIStep
	{
		[DataMember]
		public Guid SoundUid { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.PlaySound; } }
	}
}