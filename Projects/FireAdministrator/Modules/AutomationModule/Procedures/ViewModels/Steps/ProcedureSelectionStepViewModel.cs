using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace AutomationModule.ViewModels
{
	public class ProcedureSelectionStepViewModel : BaseStepViewModel
	{
		ProcedureSelectionArguments ProcedureSelectionArguments { get; set; }
		Procedure Procedure { get; set; }

		public ProcedureSelectionStepViewModel(ProcedureSelectionArguments procedureSelectionArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ProcedureSelectionArguments = procedureSelectionArguments;
			Procedure = procedure;
			UpdateContent();
		}

		public override void UpdateContent()
		{
			ScheduleProcedures = new ObservableCollection<ScheduleProcedureViewModel>();
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FindAll(x => x.Uid != Procedure.Uid))
			{
				var scheduleProcedure = new ScheduleProcedure() { ProcedureUid = procedure.Uid };
				if (procedure.Uid == ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid)
					scheduleProcedure = ProcedureSelectionArguments.ScheduleProcedure;
				ScheduleProcedures.Add(new ScheduleProcedureViewModel(scheduleProcedure));
			}
			SelectedScheduleProcedure = ScheduleProcedures.FirstOrDefault(x => x.ScheduleProcedure.ProcedureUid == ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid);
			OnPropertyChanged(() => ScheduleProcedures);
		}

		public ObservableCollection<ScheduleProcedureViewModel> ScheduleProcedures { get; private set; }
		ScheduleProcedureViewModel _selectedScheduleProcedure;
		public ScheduleProcedureViewModel SelectedScheduleProcedure
		{
			get { return _selectedScheduleProcedure; }
			set
			{
				if (value != null)
				{
					_selectedScheduleProcedure = value;
					ProcedureSelectionArguments.ScheduleProcedure = value.ScheduleProcedure;
				}
				OnPropertyChanged(() => SelectedScheduleProcedure);
			}
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}
