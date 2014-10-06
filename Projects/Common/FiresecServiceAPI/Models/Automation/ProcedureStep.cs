using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

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
			ShowMessageArguments = new ShowMessageArguments();
			ArithmeticArguments = new ArithmeticArguments();
			ConditionArguments = new ConditionArguments();
			FindObjectArguments = new FindObjectArguments();
			ForeachArguments = new ForeachArguments();
			ForArguments = new ForArguments();
			PauseArguments = new PauseArguments();
			ProcedureSelectionArguments = new ProcedureSelectionArguments();
			ExitArguments = new ExitArguments();
			SetValueArguments = new SetValueArguments();
			IncrementValueArguments = new IncrementValueArguments();
			ControlGKDeviceArguments = new ControlGKDeviceArguments();
			ControlGKFireZoneArguments = new ControlGKFireZoneArguments();
			ControlGKGuardZoneArguments = new ControlGKGuardZoneArguments();
			ControlSKDZoneArguments = new ControlSKDZoneArguments();
			ControlDirectionArguments = new ControlDirectionArguments();
			ControlDoorArguments = new ControlDoorArguments();
			ControlSKDDeviceArguments = new ControlSKDDeviceArguments();
			ControlCameraArguments = new ControlCameraArguments();
			JournalArguments = new JournalArguments();
			GetObjectPropertyArguments = new GetObjectPropertyArguments();
			SendEmailArguments = new SendEmailArguments();
			RunProgrammArguments = new RunProgrammArguments();
			RandomArguments = new RandomArguments();
			ChangeListArguments = new ChangeListArguments();
			GetListCountArgument = new GetListCountArgument();
			GetListItemArgument = new GetListItemArgument();
		}

		[XmlIgnore]
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
		public ShowMessageArguments ShowMessageArguments { get; set; }

		[DataMember]
		public ArithmeticArguments ArithmeticArguments { get; set; }

		[DataMember]
		public ConditionArguments ConditionArguments { get; set; }

		[DataMember]
		public FindObjectArguments FindObjectArguments { get; set; }

		[DataMember]
		public ForeachArguments ForeachArguments { get; set; }

		[DataMember]
		public ForArguments ForArguments { get; set; }

		[DataMember]
		public PauseArguments PauseArguments { get; set; }

		[DataMember]
		public ProcedureSelectionArguments ProcedureSelectionArguments { get; set; }

		[DataMember]
		public ExitArguments ExitArguments { get; set; }
		
		[DataMember]
		public GetListCountArgument GetListCountArgument { get; set; }

		[DataMember]
		public GetListItemArgument GetListItemArgument { get; set; }

		[DataMember]
		public SetValueArguments SetValueArguments { get; set; }

		[DataMember]
		public IncrementValueArguments IncrementValueArguments { get; set; }

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

		[DataMember]
		public GetObjectPropertyArguments GetObjectPropertyArguments { get; set; }

		[DataMember]
		public SendEmailArguments SendEmailArguments { get; set; }

		[DataMember]
		public RunProgrammArguments RunProgrammArguments { get; set; }

		[DataMember]
		public RandomArguments RandomArguments { get; set; }
		
		[DataMember]
		public ChangeListArguments ChangeListArguments { get; set; }
	}
}