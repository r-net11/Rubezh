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
		public ArithmeticParameterViewModel Variable1 { get; private set; }
		public ArithmeticParameterViewModel Result { get; private set; }

		public SetValueStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SetValueArguments = stepViewModel.Step.SetValueArguments;
			Variable1 = new ArithmeticParameterViewModel(SetValueArguments.Variable1, stepViewModel.Update);
			Result = new ArithmeticParameterViewModel(SetValueArguments.Result, stepViewModel.Update, false);
			Result.UpdateVariableHandler = UpdateVariable1;
			ExplicitTypes = new ObservableCollection<ExplicitType>(ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
			SelectedExplicitType = SetValueArguments.ExplicitType;
			UpdateContent();
		}

		void UpdateVariable1()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList));
			Variable1.ExplicitType = SelectedExplicitType;
			Variable1.EnumType = Result.EnumType;
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
			Result.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList));
			Result.ExplicitType = SelectedExplicitType;
		}

		public override string Description 
		{ 
			get { return Result.Description + " = " + Variable1.Description; } 
		}
	}
}