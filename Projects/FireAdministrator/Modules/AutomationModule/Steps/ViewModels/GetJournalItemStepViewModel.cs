using FiresecAPI;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class GetJournalItemStepViewModel : BaseStepViewModel
	{
		GetJournalItemArguments GetJournalItemArguments { get; set; }
		public ArgumentViewModel ResultArgument { get; set; }

		public GetJournalItemStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetJournalItemArguments = stepViewModel.Step.GetJournalItemArguments;
			ResultArgument = new ArgumentViewModel(GetJournalItemArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
			JournalColumnTypes = ProcedureHelper.GetEnumObs<JournalColumnType>();
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
			// Номер карты
			if (SelectedJournalColumnType == JournalColumnType.CardNo)
				explicitType = ExplicitType.Integer;
			ResultArgument.Update(Procedure, explicitType, enumType);
		}

		public ObservableCollection<JournalColumnType> JournalColumnTypes { get; private set; }
		public JournalColumnType SelectedJournalColumnType
		{
			get { return GetJournalItemArguments.JournalColumnType; }
			set
			{
				GetJournalItemArguments.JournalColumnType = value;
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