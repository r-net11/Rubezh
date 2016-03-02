using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class GetDoorPropertyStepViewModel: BaseStepViewModel
	{
		GetObjectPropertyArguments GetObjectPropertyArguments { get; set; }
		public ArgumentViewModel ObjectArgument { get; private set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public GetDoorPropertyStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			GetObjectPropertyArguments = stepViewModel.Step.GetObjectPropertyArguments;
			ObjectArgument = new ArgumentViewModel(GetObjectPropertyArguments.ObjectArgument, stepViewModel.Update, UpdateContent);
			ResultArgument = new ArgumentViewModel(GetObjectPropertyArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
			SelectedObjectType = ObjectType.Door;
		}		

		EnumType? SelectedEnumType
		{
			get
			{
				if (SelectedProperty == Property.Type)
					return EnumType.DriverType;
				if (SelectedProperty == Property.State)
					return EnumType.StateType;
				if (SelectedProperty == Property.AccessState)
					return EnumType.AccessState;
				if (SelectedProperty == Property.DoorStatus)
					return EnumType.DoorStatus;
				if (SelectedProperty == Property.BreakInStatus)
					return EnumType.BreakInStatus;
				if (SelectedProperty == Property.ConnectionStatus)
					return EnumType.ConnectionStatus;
				return null;
			}
		}

		public ObservableCollection<Property> Properties { get; private set; }
		public Property SelectedProperty
		{
			get { return GetObjectPropertyArguments.Property; }
			set
			{
				GetObjectPropertyArguments.Property = value;
				ResultArgument.Update(Procedure, ExplicitType, SelectedEnumType, isList: false);
				OnPropertyChanged(() => SelectedProperty);
			}
		}

		public override void UpdateContent()
		{
			ObjectArgument.Update(Procedure, ExplicitType.Object, objectType: SelectedObjectType, isList: false);
			ResultArgument.Update(Procedure, ExplicitType, SelectedEnumType, isList: false);
			Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(SelectedObjectType));
			OnPropertyChanged(() => Properties);
		}

		public override string Description
		{
			get 
			{ 
				return string.Format("Точка доступа: {0} Свойство: {1} Значение: {2}",ObjectArgument.Description, SelectedProperty.ToDescription(), ResultArgument.Description); 
			}
		}

		public ObjectType SelectedObjectType
		{
			get
			{
				return GetObjectPropertyArguments.ObjectType;
			}
			private set
			{
				GetObjectPropertyArguments.ObjectType = value;
				Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(SelectedObjectType));
				UpdateContent();
				OnPropertyChanged(() => Properties);
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		/// <summary>
		/// Непосредственный тип значения переменной (зависит от выбранного свойства)
		/// </summary>
		public ExplicitType ExplicitType
		{
			get { return ExplicitType.Enum; } // Все свойства в данном случае имеют тип "Перечисление"
		}
	}
}