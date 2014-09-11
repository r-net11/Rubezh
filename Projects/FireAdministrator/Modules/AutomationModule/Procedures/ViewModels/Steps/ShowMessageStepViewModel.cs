using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ShowMessageStepViewModel : BaseStepViewModel
	{
		ShowMessageArguments ShowMessageArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ShowMessageStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ShowMessageArguments = stepViewModel.Step.ShowMessageArguments;
			Variable1 = new ArithmeticParameterViewModel(ShowMessageArguments.Variable1, stepViewModel.Update);
			ExplicitTypes = new ObservableCollection<ExplicitType> (ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
			UpdateContent();
		}

		public override void UpdateContent()
		{			
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType && !x.IsList));
			Variable1.ExplicitType = ExplicitType;
		}

		public override string Description
		{
			get 
			{
				return "Сообщение: " + Variable1.Description;
			}
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType ExplicitType
		{
			get
			{
				return ShowMessageArguments.ExplicitType;
			}
			set
			{
				ShowMessageArguments.ExplicitType = value;
				UpdateContent();
				OnPropertyChanged(() => ExplicitType);
			}
		}
	}
}