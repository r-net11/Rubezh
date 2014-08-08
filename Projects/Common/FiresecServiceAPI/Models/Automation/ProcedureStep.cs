using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
			SendMessageArguments = new SendMessageArguments();
			ArithmeticArguments = new ArithmeticArguments();
			ConditionArguments = new ConditionArguments();
			FindObjectArguments = new FindObjectArguments();
			ForeachArguments = new ForeachArguments();
			PauseArguments = new PauseArguments();
			ProcedureSelectionArguments = new ProcedureSelectionArguments();
			ExitArguments = new ExitArguments();
			PersonInspectionArguments = new PersonInspectionArguments();
			SetGlobalValueArguments = new SetGlobalValueArguments();
			IncrementGlobalValueArguments = new IncrementGlobalValueArguments();
			ControlGKDeviceArguments = new ControlGKDeviceArguments();
			ControlGKFireZoneArguments = new ControlGKFireZoneArguments();
			ControlGKGuardZoneArguments = new ControlGKGuardZoneArguments();
			ControlSKDZoneArguments = new ControlSKDZoneArguments();
			ControlDirectionArguments = new ControlDirectionArguments();
			ControlDoorArguments = new ControlDoorArguments();
			ControlSKDDeviceArguments = new ControlSKDDeviceArguments();
			ControlCameraArguments = new ControlCameraArguments();
			JournalArguments = new JournalArguments();
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
		public SendMessageArguments SendMessageArguments { get; set; }

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

		[DataMember]
		public PersonInspectionArguments PersonInspectionArguments { get; set; }

		[DataMember]
		public SetGlobalValueArguments SetGlobalValueArguments { get; set; }

		[DataMember]
		public IncrementGlobalValueArguments IncrementGlobalValueArguments { get; set; }

		[DataMember]
		public ControlGKDeviceArguments ControlGKDeviceArguments { get; set; }

		[DataMember]
		public ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }

		[DataMember]
		public ControlGKGuardZoneArguments ControlGKGuardZoneArguments { get; set; }

		[DataMember]
		public ControlSKDZoneArguments ControlSKDZoneArguments { get; set; }

		[DataMember]
		public ControlDirectionArguments ControlDirectionArguments { get; set; }

		[DataMember]
		public ControlDoorArguments ControlDoorArguments { get; set; }

		[DataMember]
		public ControlSKDDeviceArguments ControlSKDDeviceArguments { get; set; }

		[DataMember]
		public ControlCameraArguments ControlCameraArguments { get; set; }

		[DataMember]
		public JournalArguments JournalArguments { get; set; }
	}
}