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
		public ArithmeticParameterViewModel MessageParameter { get; private set; }

		public ShowMessageStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ShowMessageArguments = stepViewModel.Step.ShowMessageArguments;
			MessageParameter = new ArithmeticParameterViewModel(ShowMessageArguments.MessageParameter, stepViewModel.Update);
			ExplicitTypes = new ObservableCollection<ExplicitType> (ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
			UpdateContent();
		}

		public override void UpdateContent()
		{			
			MessageParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType && !x.IsList));
			MessageParameter.ExplicitType = ExplicitType;
		}

		public override string Description
		{
			get 
			{
				return "Сообщение: " + MessageParameter.Description;
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