using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class PauseStepViewModel : BaseViewModel, IStepViewModel
	{
		private Procedure Procedure { get; set; }
		public PauseArguments PauseArguments { get; private set; }
		public ArithmeticParameterViewModel Pause { get; set; }

		public PauseStepViewModel(PauseArguments pauseArguments, Procedure procedure)
		{
			PauseArguments = pauseArguments;
			Procedure = procedure;
			TimeTypes = ProcedureHelper.GetEnumObs<TimeType>();
			Pause = new ArithmeticParameterViewModel(PauseArguments.Pause, ProcedureHelper.GetEnumList<VariableType>());
			UpdateContent();
		}

		public void UpdateContent()
		{
			Pause.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Integer));
		}

		public string Description
		{
			get { return ""; }
		}

		public ObservableCollection<TimeType> TimeTypes { get; private set; }
		public TimeType SelectedTimeType
		{
			get { return PauseArguments.TimeType; }
			set
			{
				PauseArguments.TimeType = value;
				OnPropertyChanged(() => SelectedTimeType);
			}
		}
	}
}