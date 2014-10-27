using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class GetObjectPropertyStepViewModel: BaseStepViewModel
	{
		GetObjectPropertyArguments GetObjectPropertyArguments { get; set; }
		public ArgumentViewModel ObjectArgument { get; private set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public GetObjectPropertyStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetObjectPropertyArguments = stepViewModel.Step.GetObjectPropertyArguments;
			ObjectArgument = new ArgumentViewModel(GetObjectPropertyArguments.ObjectArgument, stepViewModel.Update, UpdateContent, false);
			ObjectArgument.UpdateVariableHandler += UpdateProperies;
			ResultArgument = new ArgumentViewModel(GetObjectPropertyArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		void UpdateProperies()
		{
			Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(ObjectArgument.SelectedVariable.Variable.ObjectType));
			OnPropertyChanged(() => Properties);
		}

		public ObservableCollection<Property> Properties { get; private set; }
		public Property SelectedProperty
		{
			get { return GetObjectPropertyArguments.Property; }
			set
			{
				GetObjectPropertyArguments.Property = value;
				OnPropertyChanged(() => SelectedProperty);
				ResultArgument.Update(Procedure, ExplicitType, EnumType, isList:false);
			}
		}

		public override void UpdateContent()
		{
			ObjectArgument.Update(Procedure, ExplicitType.Object, isList:false);
			ResultArgument.Update(Procedure, ExplicitType, EnumType, isList:false);
		}

		public override string Description
		{
			get 
			{ 
				return ResultArgument.Description + " = " + ObjectArgument.Description + " Свойство: " + SelectedProperty.ToDescription(); 
			}
		}

		public EnumType EnumType
		{
			get
			{
				if (SelectedProperty == Property.Type)
					return EnumType.DriverType;
				if (SelectedProperty == Property.State)
					return EnumType.StateType;
				return EnumType.StateType;
			}
		}

		public ExplicitType ExplicitType
		{
			get
			{
				if (SelectedProperty == Property.Description || SelectedProperty == Property.Name || SelectedProperty == Property.Uid)
					return ExplicitType.String;
				if ((SelectedProperty == Property.Type) || (SelectedProperty == Property.State))
					return ExplicitType.Enum;
				return ExplicitType.Integer;
			}
		}
	}
}