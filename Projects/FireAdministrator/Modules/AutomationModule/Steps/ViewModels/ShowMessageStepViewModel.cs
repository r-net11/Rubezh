using System.Collections.ObjectModel;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ShowMessageStepViewModel : BaseStepViewModel
	{
		ShowMessageArguments ShowMessageArguments { get; set; }
		public ArgumentViewModel MessageArgument { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }

		public ShowMessageStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ShowMessageArguments = stepViewModel.Step.ShowMessageArguments;
			MessageArgument = new ArgumentViewModel(ShowMessageArguments.MessageArgument, stepViewModel.Update, null);
			ExplicitTypes = new ObservableCollection<ExplicitType> (ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>(); 
		}

		public override void UpdateContent()
		{
			MessageArgument.Update(Procedure, ExplicitType, EnumType, isList:false);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowMessageArguments.LayoutFilter);
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
	}
}