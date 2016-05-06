using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.Automation;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ProcedureSelectionViewModel : SaveCancelDialogViewModel
	{
		public ObservableCollection<ProcedureViewModel> Procedures { get; private set; }
		public ProcedureSelectionViewModel()
		{
			Title = "Выбор процедуры";
			Procedures = new ObservableCollection<ProcedureViewModel>();
			if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures == null)
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures = new List<Procedure>();
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				var procedureViewModel = new ProcedureViewModel(procedure);
				Procedures.Add(procedureViewModel);
			}
			SelectedProcedure = Procedures.FirstOrDefault();
		}

		public ProcedureSelectionViewModel(Guid procedureUid)
		{
			Title = "Выбор процедуры";
			Procedures = new ObservableCollection<ProcedureViewModel>();
			if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures == null)
				FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures = new List<Procedure>();
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FindAll(x => x.Uid != procedureUid))
			{
				var procedureViewModel = new ProcedureViewModel(procedure);
				Procedures.Add(procedureViewModel);
			}
			SelectedProcedure = Procedures.FirstOrDefault();
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
