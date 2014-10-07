using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class SetValueStepViewModel : BaseStepViewModel
	{
		SetValueArguments SetValueArguments { get; set; }
		public ArgumentViewModel SourceArgument { get; private set; }
		public ArgumentViewModel TargetArgument { get; private set; }

		public SetValueStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SetValueArguments = stepViewModel.Step.SetValueArguments;
			SourceArgument = new ArgumentViewModel(SetValueArguments.SourceArgument, stepViewModel.Update);
			TargetArgument = new ArgumentViewModel(SetValueArguments.TargetArgument, stepViewModel.Update, false);
			ExplicitTypes = ProcedureHelper.GetEnumObs<ExplicitType>();
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			SelectedExplicitType = SetValueArguments.ExplicitType;
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }

		public ExplicitType SelectedExplicitType
		{
			get { return SetValueArguments.ExplicitType; }
			set
			{
				SetValueArguments.ExplicitType = value;
				OnPropertyChanged(() => SelectedExplicitType);
				UpdateContent();
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }

		public EnumType SelectedEnumType
		{
			get
			{
				return SetValueArguments.EnumType;
			}
			set
			{
				SetValueArguments.EnumType = value;
				UpdateContent();
				OnPropertyChanged(() => SelectedEnumType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }

		public ObjectType SelectedObjectType
		{
			get
			{
				return SetValueArguments.ObjectType;
			}
			set
			{
				SetValueArguments.ObjectType = value;
				UpdateContent();
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		public override void UpdateContent()
		{
			List<Variable> allVariables;
			if (SelectedExplicitType == ExplicitType.Enum)
			{
				allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList && x.EnumType == SelectedEnumType);
				TargetArgument.EnumType = SelectedEnumType;
				SourceArgument.EnumType = SelectedEnumType;
			}
			else if (SelectedExplicitType == ExplicitType.Object)
			{
				allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList && x.ObjectType == SelectedObjectType);
				TargetArgument.ObjectType = SelectedObjectType;
				SourceArgument.ObjectType = SelectedObjectType;
			}
			else
				allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList);
			TargetArgument.ExplicitType = SelectedExplicitType;
			SourceArgument.ExplicitType = SelectedExplicitType;
			TargetArgument.Update(allVariables);
			SourceArgument.Update(allVariables);
		}

		public override string Description 
		{ 
			get { return TargetArgument.Description + " = " + SourceArgument.Description; } 
		}
	}
}