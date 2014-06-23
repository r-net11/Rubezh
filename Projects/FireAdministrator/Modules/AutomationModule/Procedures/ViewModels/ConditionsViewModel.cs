using System.Collections.Generic;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ConditionsViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }
		public List<FilterViewModel> Filtes { get; private set; }
		public ConditionsViewModel(Procedure procedure)
		{
			Procedure = procedure;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var procedureDetailsViewModel = new ProcedureDetailsViewModel();
			if (DialogService.ShowModalWindow(procedureDetailsViewModel))
			{
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Add(procedureDetailsViewModel.Procedure);
				var procedureViewModel = new ProcedureViewModel(procedureDetailsViewModel.Procedure);
				//Procedures.Add(procedureViewModel);
				//SelectedProcedure = procedureViewModel;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			//FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Remove(SelectedProcedure.Procedure);
			//Procedures.Remove(SelectedProcedure);
			//SelectedProcedure = Procedures.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = true;
		}
		bool CanDelete()
		{
			return true;
			//return SelectedProcedure != null;
		}
	}
}