using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ProcedureStep
	{
		public ProcedureStep()
		{
			UID = Guid.NewGuid();
			Children = new List<ProcedureStep>();
			SoundArguments = new SoundArguments();
			ArithmeticArguments = new ArithmeticArguments();
			ConditionArguments = new ConditionArguments();
			FindObjectArguments = new FindObjectArguments();
			ForeachArguments = new ForeachArguments();
			PauseArguments = new PauseArguments();
			ProcedureSelectionArguments = new ProcedureSelectionArguments();
			ExitArguments = new ExitArguments();
		}

		public ProcedureStep Parent { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<ProcedureStep> Children { get; set; }

		[DataMember]
		public ProcedureStepType ProcedureStepType { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public SoundArguments SoundArguments { get; set; }

		[DataMember]
		public ArithmeticArguments ArithmeticArguments { get; set; }

		[DataMember]
		public ConditionArguments ConditionArguments { get; set; }

		[DataMember]
		public FindObjectArguments FindObjectArguments { get; set; }

		[DataMember]
		public ForeachArguments ForeachArguments { get; set; }

		[DataMember]
		public PauseArguments PauseArguments { get; set; }
		
		[DataMember]
		public ProcedureSelectionArguments ProcedureSelectionArguments { get; set; }

		[DataMember]
		public ExitArguments ExitArguments { get; set; }
	}
}