using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ExportJournalStepViewModel : BaseStepViewModel
	{
		ExportJournalArguments ExportJournalArguments { get; set; }
		public ArgumentViewModel IsExportJournalArgument { get; private set; }
		public ArgumentViewModel IsExportPassJournalArgument { get; private set; }
		public ArgumentViewModel MinDateArgument { get; private set; }
		public ArgumentViewModel MaxDateArgument { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ExportJournalStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExportJournalArguments = stepViewModel.Step.ExportJournalArguments;
			IsExportJournalArgument = new ArgumentViewModel(ExportJournalArguments.IsExportJournalArgument, stepViewModel.Update, UpdateContent);
			IsExportPassJournalArgument = new ArgumentViewModel(ExportJournalArguments.IsExportPassJournalArgument, stepViewModel.Update, UpdateContent);
			MinDateArgument = new ArgumentViewModel(ExportJournalArguments.MinDateArgument, stepViewModel.Update, UpdateContent);
			MaxDateArgument = new ArgumentViewModel(ExportJournalArguments.MaxDateArgument, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ExportJournalArguments.PathArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IsExportJournalArgument.Update(Procedure, ExplicitType.Boolean);
			IsExportPassJournalArgument.Update(Procedure, ExplicitType.Boolean);
			MinDateArgument.Update(Procedure, ExplicitType.DateTime);
			MaxDateArgument.Update(Procedure, ExplicitType.DateTime);
			PathArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
				var result = "Экспортировать ";
				if (IsExportJournalArgument.ExplicitValue.BoolValue && !IsExportPassJournalArgument.ExplicitValue.BoolValue)
					result += "журнал событий ";
				else if (!IsExportJournalArgument.ExplicitValue.BoolValue && IsExportPassJournalArgument.ExplicitValue.BoolValue)
					result += "журнал проходов ";
				else if (IsExportJournalArgument.ExplicitValue.BoolValue && IsExportPassJournalArgument.ExplicitValue.BoolValue)
					result += "журнал событий и журнал проходов ";
				result += "c " + MinDateArgument.Description + " до " + MaxDateArgument.Description;
				if (!PathArgument.IsEmpty)
					result += "в " + PathArgument.Description;
				return result;
			}
		}
	}
}
