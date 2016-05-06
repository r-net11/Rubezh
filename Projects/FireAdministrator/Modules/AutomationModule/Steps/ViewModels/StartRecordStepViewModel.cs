using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class StartRecordStepViewModel : BaseStepViewModel
	{
		StartRecordStep StartRecordStep { get; set; }
		public ArgumentViewModel CameraArgument { get; private set; }
		public ArgumentViewModel EventUIDArgument { get; set; }
		public ArgumentViewModel TimeoutArgument { get; set; }

		public StartRecordStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			StartRecordStep = (StartRecordStep)stepViewModel.Step;
			EventUIDArgument = new ArgumentViewModel(StartRecordStep.EventUIDArgument, stepViewModel.Update, UpdateContent, false);
			TimeoutArgument = new ArgumentViewModel(StartRecordStep.TimeoutArgument, stepViewModel.Update, UpdateContent);
			CameraArgument = new ArgumentViewModel(StartRecordStep.CameraArgument, stepViewModel.Update, null);
			TimeTypes = AutomationHelper.GetEnumObs<TimeType>();
			SelectedTimeType = StartRecordStep.TimeType;
		}

		public ObservableCollection<TimeType> TimeTypes { get; private set; }
		public TimeType SelectedTimeType
		{
			get { return StartRecordStep.TimeType; }
			set
			{
				StartRecordStep.TimeType = value;
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