using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;

namespace AutomationModule.ViewModels
{
	public class IncrementValueStepViewModel: BaseStepViewModel
	{
		IncrementValueArguments IncrementValueArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public IncrementValueStepViewModel(IncrementValueArguments incrementGlobalValueArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			IncrementValueArguments = incrementGlobalValueArguments;
			Procedure = procedure;
			IncrementTypes = new ObservableCollection<IncrementType> { IncrementType.Inc, IncrementType.Dec };
			Variable1 = new ArithmeticParameterViewModel(IncrementValueArguments.Variable1, false);
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

		public override void UpdateContent()
		{			
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == FiresecAPI.Automation.ValueType.Integer));
		}

		public override string Description { get { return ""; } }
	}
}
