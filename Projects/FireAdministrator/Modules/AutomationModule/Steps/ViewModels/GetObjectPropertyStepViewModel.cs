﻿using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;
using Infrastructure.Automation;

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
			ObjectArgument = new ArgumentViewModel(GetObjectPropertyArguments.ObjectArgument, stepViewModel.Update, UpdateContent);
			ObjectTypes = AutomationHelper.GetEnumObs<ObjectType>();
			ResultArgument = new ArgumentViewModel(GetObjectPropertyArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}		

		EnumType? SelectedEnumType
		{
			get
			{
				if (SelectedProperty == Property.Type)
					return EnumType.DriverType;
				if (SelectedProperty == Property.State)
					return EnumType.StateType;
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
			Properties = new ObservableCollection<Property>(AutomationHelper.ObjectTypeToProperiesList(SelectedObjectType));
			OnPropertyChanged(() => Properties);
		}

		public override string Description
		{
			get 
			{ 
				return ResultArgument.Description + " = " + ObjectArgument.Description + " Свойство: " + SelectedProperty.ToDescription(); 
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType SelectedObjectType
		{
			get
			{
				return GetObjectPropertyArguments.ObjectType;
			}
			set
			{
				GetObjectPropertyArguments.ObjectType = value;
				Properties = new ObservableCollection<Property>(AutomationHelper.ObjectTypeToProperiesList(SelectedObjectType));
				UpdateContent();
				OnPropertyChanged(() => Properties);
				OnPropertyChanged(() => SelectedObjectType);
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