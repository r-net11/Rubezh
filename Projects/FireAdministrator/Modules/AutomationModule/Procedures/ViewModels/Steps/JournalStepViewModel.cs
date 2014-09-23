using System;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class JournalStepViewModel : BaseStepViewModel
	{
		JournalArguments JournalArguments { get; set; }
		public ArgumentViewModel MessageParameter { get; set; }

		public JournalStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			JournalArguments = stepViewModel.Step.JournalArguments;
			MessageParameter = new ArgumentViewModel(JournalArguments.MessageParameter, stepViewModel.Update);
			ExplicitTypes = new ObservableCollection<ExplicitType>(ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>(); 
			UpdateContent();
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType ExplicitType
		{
			get { return JournalArguments.ExplicitType; }
			set
			{
				JournalArguments.ExplicitType = value;				
				UpdateContent();
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType EnumType
		{
			get
			{
				return JournalArguments.EnumType;
			}
			set
			{
				JournalArguments.EnumType = value;
				UpdateContent();
				OnPropertyChanged(() => EnumType);
			}
		}

		public override void UpdateContent()
		{
			var allVariables = new List<Variable>();
			if (ExplicitType == ExplicitType.Enum)
				allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType && !x.IsList && x.EnumType == EnumType);
			else
				allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType && !x.IsList);
			MessageParameter.Update(allVariables);
			MessageParameter.ExplicitType = ExplicitType;
			MessageParameter.SelectedEnumType = EnumType;
		}

		public override string Description
		{
			get 
			{ 
				return "Сообщение: " + MessageParameter.Description; 
			}
		}
	}
}