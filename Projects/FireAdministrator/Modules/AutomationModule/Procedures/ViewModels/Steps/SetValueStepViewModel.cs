using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;

namespace AutomationModule.ViewModels
{
	public class SetValueStepViewModel : BaseStepViewModel
	{
		SetValueArguments SetValueArguments { get; set; }
		public ArithmeticParameterViewModel SourceParameter { get; private set; }
		public ArithmeticParameterViewModel TargetParameter { get; private set; }

		public SetValueStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SetValueArguments = stepViewModel.Step.SetValueArguments;
			SourceParameter = new ArithmeticParameterViewModel(SetValueArguments.SourceParameter, stepViewModel.Update);
			TargetParameter = new ArithmeticParameterViewModel(SetValueArguments.TargetParameter, stepViewModel.Update, false);
			TargetParameter.UpdateVariableHandler = UpdateSourceParameter;
			ExplicitTypes = new ObservableCollection<ExplicitType>(ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
			SelectedExplicitType = SetValueArguments.ExplicitType;
			UpdateContent();
		}

		void UpdateSourceParameter()
		{
			SourceParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList));
			SourceParameter.EnumType = TargetParameter.EnumType;
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

		public override void UpdateContent()
		{
			TargetParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList));
			TargetParameter.ExplicitType = SelectedExplicitType;
			SourceParameter.ExplicitType = SelectedExplicitType;
		}

		public override string Description 
		{ 
			get { return TargetParameter.Description + " = " + SourceParameter.Description; } 
		}
	}
}