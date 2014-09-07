using System;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using System.Linq;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class JournalStepViewModel : BaseViewModel, IStepViewModel
	{
		JournalArguments JournalArguments { get; set; }
		public Action UpdateDescriptionHandler { get; set; }
		public ArithmeticParameterViewModel Variable { get; set; }
		Procedure Procedure { get; set; }

		public JournalStepViewModel(JournalArguments journalArguments, Action updateDescriptionHandler, Procedure procedure)
		{
			JournalArguments = journalArguments;
			Procedure = procedure;
			Variable = new ArithmeticParameterViewModel(journalArguments.Variable);
			ValueTypes = new ObservableCollection<ValueType>(Enum.GetValues(typeof(ValueType)).Cast<ValueType>().ToList().FindAll(x => x != ValueType.Object));
			SelectedValueType = JournalArguments.ValueType;
			UpdateDescriptionHandler = updateDescriptionHandler;
			UpdateContent();
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		public ValueType SelectedValueType
		{
			get { return JournalArguments.ValueType; }
			set
			{
				JournalArguments.ValueType = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedValueType);
				UpdateContent();
			}
		}

		public void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList);
			if (SelectedValueType == ValueType.Boolean)
			{
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.Boolean);
			}
			if (SelectedValueType == ValueType.Integer)
			{
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.Integer);
			}
			if (SelectedValueType == ValueType.DateTime)
			{
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.DateTime);
			}
			if (SelectedValueType == ValueType.String)
			{
				allVariables = allVariables.FindAll(x => x.ValueType == ValueType.String);
			}
			Variable.Update(allVariables);
		}

		public string Description
		{
			get { return ""; }
		}
	}
}