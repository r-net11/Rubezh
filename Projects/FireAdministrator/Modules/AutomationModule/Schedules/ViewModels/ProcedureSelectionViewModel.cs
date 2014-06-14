using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ProcedureSelectionViewModel : SaveCancelDialogViewModel
	{
		public ObservableCollection<ProcedureViewModel> Procedures { get; private set; }
		public ProcedureSelectionViewModel(ObservableCollection<ProcedureViewModel> procedures)
		{
			Initialize(procedures);
		}

		public void Initialize(ObservableCollection<ProcedureViewModel> procedures)
		{
			Procedures = new ObservableCollection<ProcedureViewModel>();
			if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures == null)
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures = new List<Procedure>();
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				if (procedures.Any(x => x.Procedure.Uid == procedure.Uid))
					continue;
				var procedureViewModel = new ProcedureViewModel(procedure);
				Procedures.Add(procedureViewModel);
			}
		}

		private ProcedureViewModel _selectedProcedure;
		public ProcedureViewModel SelectedProcedure
		{
			get { return _selectedProcedure; }
			set
			{
				_selectedProcedure = value;
				OnPropertyChanged(() => SelectedProcedure);
			}
		}
	}
}
