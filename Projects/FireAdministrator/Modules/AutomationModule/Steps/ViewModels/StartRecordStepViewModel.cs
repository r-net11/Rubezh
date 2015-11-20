using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using RubezhAPI;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class StartRecordStepViewModel : BaseStepViewModel
	{
		StartRecordArguments StartRecordArguments { get; set; }
		public ArgumentViewModel CameraArgument { get; private set; }
		public ArgumentViewModel EventUIDArgument { get; set; }
		public ArgumentViewModel TimeoutArgument { get; set; }

		public StartRecordStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			StartRecordArguments = stepViewModel.Step.StartRecordArguments;
			EventUIDArgument = new ArgumentViewModel(StartRecordArguments.EventUIDArgument, stepViewModel.Update, UpdateContent);
			TimeoutArgument = new ArgumentViewModel(StartRecordArguments.TimeoutArgument, stepViewModel.Update, UpdateContent);
			CameraArgument = new ArgumentViewModel(StartRecordArguments.CameraArgument, stepViewModel.Update, null);
			TimeTypes = AutomationHelper.GetEnumObs<TimeType>();
			SelectedTimeType = StartRecordArguments.TimeType;
		}

		public ObservableCollection<TimeType> TimeTypes { get; private set; }
		public TimeType SelectedTimeType
		{
			get { return StartRecordArguments.TimeType; }
			set
			{
				StartRecordArguments.TimeType = value;
				OnPropertyChanged(() => SelectedTimeType);
			}
		}

		public override void UpdateContent()
		{
			CameraArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.VideoDevice, isList: false);
			EventUIDArgument.Update(Procedure, ExplicitType.String);
			TimeoutArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description
		{
			get
			{
				return string.Format("Камера: {0} Идентификатор: {1} Длительность: {2} {3}", 
					CameraArgument.Description, 
					EventUIDArgument.Description, 
					TimeoutArgument.Description, 
					SelectedTimeType.ToDescription());
			}
		}
	}
}