using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class IncrementValueStepViewModel: BaseViewModel, IStepViewModel
	{
		IncrementValueArguments IncrementValueArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public IncrementValueStepViewModel(IncrementValueArguments incrementGlobalValueArguments, Procedure procedure)
		{
			IncrementValueArguments = incrementGlobalValueArguments;
			Procedure = procedure;
			IncrementTypes = new ObservableCollection<IncrementType> { IncrementType.Inc, IncrementType.Dec };
			Variable1 = new ArithmeticParameterViewModel(IncrementValueArguments.Variable1, new List<VariableType> { VariableType.IsGlobalVariable, VariableType.IsLocalVariable });
			UpdateContent();
		}
		
		public ObservableCollection<IncrementType> IncrementTypes { get; private set; }
		public IncrementType SelectedIncrementType
		{
			get { return IncrementValueArguments.IncrementType; }
			set
			{
				IncrementValueArguments.IncrementType = value;
				OnPropertyChanged(() => SelectedIncrementType);
			}
		}

		public void UpdateContent()
		{			
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == ValueType.Integer));
		}

		public string Description { get { return ""; } }
	}
}
