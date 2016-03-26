using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class CloseDialogStepViewModel : BaseStepViewModel
	{
		public CloseDialogStep CloseDialogStep { get; private set; }
		public ArgumentViewModel WindowUIDArgument { get; private set; }

		public CloseDialogStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			CloseDialogStep = (CloseDialogStep)stepViewModel.Step;
			IsServerContext = Procedure.ContextType == ContextType.Server;
			WindowUIDArgument = new ArgumentViewModel(CloseDialogStep.WindowIDArgument, stepViewModel.Update, UpdateContent);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(CloseDialogStep.LayoutFilter);
			IsServerContext = Procedure.ContextType == ContextType.Server;
		}

		public bool ForAllClients
		{
			get { return CloseDialogStep.ForAllClients; }
			set
			{
				CloseDialogStep.ForAllClients = value;
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
				return "Закрыть диалог: ID=" + WindowUIDArgument.Description;
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
			WindowUIDArgument.Update(Procedure, ExplicitType.String, isList: false);
		}
	}
}
