using Infrastructure.Automation;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ShowPropertyStepViewModel : BaseStepViewModel
	{
		ShowPropertyStep ShowPropertyStep { get; set; }
		public ArgumentViewModel ObjectArgument { get; private set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }

		public ShowPropertyStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ShowPropertyStep = (ShowPropertyStep)stepViewModel.Step;
			ObjectArgument = new ArgumentViewModel(ShowPropertyStep.ObjectArgument, stepViewModel.Update, null);
			ObjectTypes = new ObservableCollection<ObjectType>(AutomationHelper.GetEnumList<ObjectType>().FindAll(x => x != ObjectType.Organisation));
			IsServerContext = Procedure.ContextType == ContextType.Server;
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
			ObjectArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType, isList: false);
			IsServerContext = Procedure.ContextType == ContextType.Server;
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowPropertyStep.LayoutFilter);
			OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
		}

		public override string Description
		{
			get
			{
				return "Показать свойство объекта: " + ObjectArgument.Description;
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType ObjectType
		{
			get
			{
				return ShowPropertyStep.ObjectType;
			}
			set
			{
				ShowPropertyStep.ObjectType = value;
				UpdateContent();
				OnPropertyChanged(() => ObjectType);
			}
		}

		public bool ForAllClients
		{
			get { return ShowPropertyStep.ForAllClients; }
			set
			{
				ShowPropertyStep.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}
	}
}