using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class GetJournalItemStepViewModel : BaseStepViewModel
	{
		GetJournalItemStep GetJournalItemStep { get; set; }
		public ArgumentViewModel ResultArgument { get; set; }

		public GetJournalItemStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			GetJournalItemStep = (GetJournalItemStep)stepViewModel.Step;
			ResultArgument = new ArgumentViewModel(GetJournalItemStep.ResultArgument, stepViewModel.Update, UpdateContent, false);
			JournalColumnTypes = AutomationHelper.GetEnumObs<JournalColumnType>();
		}

		public override void UpdateContent()
		{
			var explicitType = ExplicitType.Enum;
			var enumType = new EnumType();
			if (SelectedJournalColumnType == JournalColumnType.JournalEventNameType)
				enumType = EnumType.JournalEventNameType;
			if (SelectedJournalColumnType == JournalColumnType.JournalEventDescriptionType)
				enumType = EnumType.JournalEventDescriptionType;
			if (SelectedJournalColumnType == JournalColumnType.JournalObjectType)
				enumType = EnumType.JournalObjectType;
			if (SelectedJournalColumnType == JournalColumnType.DeviceDateTime || SelectedJournalColumnType == JournalColumnType.SystemDateTime)
				explicitType = ExplicitType.DateTime;
			if (SelectedJournalColumnType == JournalColumnType.JournalObjectUid)
				explicitType = ExplicitType.String;
			ResultArgument.Update(Procedure, explicitType, enumType);
		}

		public ObservableCollection<JournalColumnType> JournalColumnTypes { get; private set; }
		public JournalColumnType SelectedJournalColumnType
		{
			get { return GetJournalItemStep.JournalColumnType; }
			set
			{
				GetJournalItemStep.JournalColumnType = value;
				OnPropertyChanged(() => SelectedJournalColumnType);
				UpdateContent();
			}
		}

		public override string Description
		{
			get
			{
				return "Получить значение колонки: " + SelectedJournalColumnType.ToDescription() + " Результат: " + ResultArgument.Description;
			}
		}
	}
}