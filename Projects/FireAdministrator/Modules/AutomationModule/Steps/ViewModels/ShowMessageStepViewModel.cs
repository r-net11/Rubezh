using Infrastructure.Automation;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ShowMessageStepViewModel : BaseStepViewModel
	{
		ShowMessageStep ShowMessageStep { get; set; }
		public ArgumentViewModel MessageArgument { get; private set; }
		public ArgumentViewModel ConfirmationValueArgument { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }

		public ShowMessageStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ShowMessageStep = (ShowMessageStep)stepViewModel.Step;
			MessageArgument = new ArgumentViewModel(ShowMessageStep.MessageArgument, stepViewModel.Update, UpdateContent);
			ConfirmationValueArgument = new ArgumentViewModel(ShowMessageStep.ConfirmationValueArgument, stepViewModel.Update, UpdateContent, false);
			ExplicitTypes = new ObservableCollection<ExplicitType>(AutomationHelper.GetEnumList<ExplicitType>());
			EnumTypes = AutomationHelper.GetEnumObs<EnumType>();
			IsServerContext = Procedure.ContextType == ContextType.Server;
		}

		public override void UpdateContent()
		{
			MessageArgument.Update(Procedure, ExplicitType, EnumType, isList: false);
			ConfirmationValueArgument.Update(Procedure, ExplicitType.Boolean, isList: false);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowMessageStep.LayoutFilter);
			IsServerContext = Procedure.ContextType == ContextType.Server;
			OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
		}

		public bool IsModalWindow
		{
			get { return ShowMessageStep.IsModalWindow; }
			set
			{
				ShowMessageStep.IsModalWindow = value;
				OnPropertyChanged(() => IsModalWindow);
			}
		}

		public bool WithConfirmation
		{
			get { return ShowMessageStep.WithConfirmation; }
			set
			{
				ShowMessageStep.WithConfirmation = value;
				OnPropertyChanged(() => WithConfirmation);
			}
		}

		public override string Description
		{
			get
			{
				return string.Format("Сообщение: {0} {1}; Подтверждение = {2}", MessageArgument == null ? ArgumentViewModel.EmptyText : MessageArgument.Description, IsModalWindow ? "(модальное)" : "(не модальное)", WithConfirmation == false ? "Нет" : ConfirmationValueArgument.Description);
			}
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType ExplicitType
		{
			get
			{
				return ShowMessageStep.ExplicitType;
			}
			set
			{
				ShowMessageStep.ExplicitType = value;
				UpdateContent();
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType EnumType
		{
			get
			{
				return ShowMessageStep.EnumType;
			}
			set
			{
				ShowMessageStep.EnumType = value;
				UpdateContent();
				OnPropertyChanged(() => EnumType);
			}
		}

		public bool ForAllClients
		{
			get { return ShowMessageStep.ForAllClients; }
			set
			{
				ShowMessageStep.ForAllClients = value;
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