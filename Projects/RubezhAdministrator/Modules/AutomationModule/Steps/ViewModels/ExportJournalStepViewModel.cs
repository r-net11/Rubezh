using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ExportJournalStepViewModel : BaseStepViewModel
	{
		ExportJournalStep ExportJournalStep { get; set; }
		public ArgumentViewModel IsExportJournalArgument { get; private set; }
		public ArgumentViewModel IsExportPassJournalArgument { get; private set; }
		public ArgumentViewModel MinDateArgument { get; private set; }
		public ArgumentViewModel MaxDateArgument { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ExportJournalStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExportJournalStep = (ExportJournalStep)stepViewModel.Step;
			IsExportJournalArgument = new ArgumentViewModel(ExportJournalStep.IsExportJournalArgument, stepViewModel.Update, UpdateContent);
			IsExportPassJournalArgument = new ArgumentViewModel(ExportJournalStep.IsExportPassJournalArgument, stepViewModel.Update, UpdateContent);
			MinDateArgument = new ArgumentViewModel(ExportJournalStep.MinDateArgument, stepViewModel.Update, UpdateContent);
			MaxDateArgument = new ArgumentViewModel(ExportJournalStep.MaxDateArgument, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ExportJournalStep.PathArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IsExportJournalArgument.Update(Procedure, ExplicitType.Boolean, isList: false);
			IsExportPassJournalArgument.Update(Procedure, ExplicitType.Boolean, isList: false);
			MinDateArgument.Update(Procedure, ExplicitType.DateTime, isList: false);
			MaxDateArgument.Update(Procedure, ExplicitType.DateTime, isList: false);
			PathArgument.Update(Procedure, ExplicitType.String, isList: false);
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
