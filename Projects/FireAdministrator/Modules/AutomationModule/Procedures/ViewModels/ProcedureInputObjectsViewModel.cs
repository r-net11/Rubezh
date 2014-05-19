using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecAPI.Models;

namespace AutomationModule.ViewModels
{
	public class ProcedureInputObjectsViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }

		public ProcedureInputObjectsViewModel(Procedure procedure)
		{
			Procedure = procedure;
			DeleteCommand = new RelayCommand(OnDelete, CanDeleted);
			AddCommand = new RelayCommand(OnAdd);
			InputObjects = new ObservableCollection<ProcedureInputObjectViewModel>();
			foreach (var inputObject in procedure.InputObjects)
			{
				var procedureInputObjectViewModel = new ProcedureInputObjectViewModel(inputObject);
				InputObjects.Add(procedureInputObjectViewModel);
			}
		}

		public ObservableCollection<ProcedureInputObjectViewModel> InputObjects { get; private set; }

		ProcedureInputObjectViewModel _selectedInputObject;
		public ProcedureInputObjectViewModel SelectedInputObject
		{
			get { return _selectedInputObject; }
			set
			{
				_selectedInputObject = value;
				OnPropertyChanged("SelectedInputObject");
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Procedure.InputObjects.Remove(SelectedInputObject.ProcedureInputObject);
			InputObjects.Remove(SelectedInputObject);
		}
		bool CanDeleted()
		{
			return SelectedInputObject != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var procedureObjectsSelectationViewModel = new ProcedureObjectsSelectationViewModel();
			if (DialogService.ShowModalWindow(procedureObjectsSelectationViewModel))
			{
				Procedure.InputObjects.Add(procedureObjectsSelectationViewModel.ProcedureInputObject);
				var procedureInputObjectViewModel = new ProcedureInputObjectViewModel(procedureObjectsSelectationViewModel.ProcedureInputObject);
				InputObjects.Add(procedureInputObjectViewModel);
			}
		}
	}
}