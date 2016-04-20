using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using System.Collections.Generic;
using System.Linq;

namespace PlansModule.ViewModels
{
	public class PlanPropertyBindingViewModel : SaveCancelDialogViewModel
	{
		PlanElementBindingItem PlanElementBindingItem;

		public PlanPropertyBindingViewModel(PlanElementBindingItem planElementBindingItem)
		{
			Title = "Выбор привязки";

			PlanElementBindingItem = planElementBindingItem;
			GlobalVariables = ClientManager.SystemConfiguration.AutomationConfiguration.GlobalVariables;
			SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.Uid == planElementBindingItem.GlobalVariableUID);
		}

		public List<Variable> GlobalVariables { get; private set; }

		Variable _selectedGlobalVariable;
		public Variable SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				OnPropertyChanged(() => SelectedGlobalVariable);
			}
		}

		protected override bool Save()
		{
			PlanElementBindingItem.GlobalVariableUID = SelectedGlobalVariable.Uid;
			return base.Save();
		}
	}
}