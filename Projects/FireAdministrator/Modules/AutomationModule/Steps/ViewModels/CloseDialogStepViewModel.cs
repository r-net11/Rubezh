using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class CloseDialogStepViewModel : BaseStepViewModel
	{
		public CloseDialogArguments CloseDialogArguments { get; private set; }
		public ArgumentViewModel WindowUIDArgument { get; private set; }

		public CloseDialogStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			CloseDialogArguments = stepViewModel.Step.CloseDialogArguments;
			IsServerContext = Procedure.ContextType == ContextType.Server;
			WindowUIDArgument = new ArgumentViewModel(CloseDialogArguments.WindowIDArgument, stepViewModel.Update, UpdateContent);
			WindowUIDArgument.Update(Procedure, ExplicitType.String);
		}

		public bool ForAllClients
		{
			get { return CloseDialogArguments.ForAllClients; }
			set
			{
				CloseDialogArguments.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}

		private ProcedureLayoutCollectionViewModel _procedureLayoutCollectionViewModel;
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel
		{
			get { return _procedureLayoutCollectionViewModel; }
			private set
			{
				_procedureLayoutCollectionViewModel = value;
				OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
			}
		}

		public override string Description
		{
			get
			{
				return string.Format("Закрыть диалог");
			}
		}

		bool _isServerContext;
		public bool IsServerContext
		{
			get { return _isServerContext; }
			set
			{
				_isServerContext = value;
				OnPropertyChanged(() => IsServerContext);
			}
		}
		public override void UpdateContent()
		{
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(CloseDialogArguments.LayoutFilter);
			IsServerContext = Procedure.ContextType == ContextType.Server;
		}
	}
}
