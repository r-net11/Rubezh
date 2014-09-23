using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class IncrementValueStepViewModel: BaseStepViewModel
	{
		IncrementValueArguments IncrementValueArguments { get; set; }
		public ArgumentViewModel ResultParameter { get; private set; }

		public IncrementValueStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			IncrementValueArguments = stepViewModel.Step.IncrementValueArguments;
			IncrementTypes = new ObservableCollection<IncrementType> { IncrementType.Inc, IncrementType.Dec };
			ResultParameter = new ArgumentViewModel(IncrementValueArguments.ResultParameter, stepViewModel.Update, false);
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
			ResultParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == FiresecAPI.Automation.ExplicitType.Integer));
		}

		public override string Description 
		{
			get 
			{
				return "Переменная: " + ResultParameter.Description + " Значение: " + SelectedIncrementType.ToDescription();
			}
		}
	}
}
