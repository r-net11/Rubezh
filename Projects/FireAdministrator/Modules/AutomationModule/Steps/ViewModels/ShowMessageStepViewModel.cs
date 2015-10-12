using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class ShowMessageStepViewModel : BaseStepViewModel
	{
		ShowMessageArguments ShowMessageArguments { get; set; }
		public ArgumentViewModel MessageArgument { get; private set; }
		public ArgumentViewModel ConfirmationValueArgument { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }
		
		public ShowMessageStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ShowMessageArguments = stepViewModel.Step.ShowMessageArguments;
			MessageArgument = new ArgumentViewModel(ShowMessageArguments.MessageArgument, stepViewModel.Update, null);
			ConfirmationValueArgument = new ArgumentViewModel(ShowMessageArguments.ConfirmationValueArgument, stepViewModel.Update, null, false);
			ExplicitTypes = new ObservableCollection<ExplicitType>(AutomationHelper.GetEnumList<ExplicitType>());
			EnumTypes = AutomationHelper.GetEnumObs<EnumType>();
			IsServerContext = Procedure.ContextType == ContextType.Server;
		}

		public override void UpdateContent()
		{
			MessageArgument.Update(Procedure, ExplicitType, EnumType, isList: false);
			ConfirmationValueArgument.Update(Procedure, ExplicitType.Boolean, isList: false);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowMessageArguments.LayoutFilter);
			IsServerContext = Procedure.ContextType == ContextType.Server;
			OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
		}

		public bool IsModalWindow
		{
			get { return ShowMessageArguments.IsModalWindow; }
			set
			{
				ShowMessageArguments.IsModalWindow = value;
				OnPropertyChanged(() => IsModalWindow);
			}
		}

		public bool WithConfirmation
		{
			get { return ShowMessageArguments.WithConfirmation; }
			set
			{
				ShowMessageArguments.WithConfirmation = value;
				OnPropertyChanged(() => WithConfirmation);
			}
		}

		public override string Description
		{
			get
			{
				return "Сообщение: " + MessageArgument.Description;
			}
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType ExplicitType
		{
			get
			{
				return ShowMessageArguments.ExplicitType;
			}
			set
			{
				ShowMessageArguments.ExplicitType = value;
				UpdateContent();
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType EnumType
		{
			get
			{
				return ShowMessageArguments.EnumType;
			}
			set
			{
				ShowMessageArguments.EnumType = value;
				UpdateContent();
				OnPropertyChanged(() => EnumType);
			}
		}

		public bool ForAllClients
		{
			get { return ShowMessageArguments.ForAllClients; }
			set
			{
				ShowMessageArguments.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
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
	}
}