using Infrastructure;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;

namespace AutomationModule.ViewModels
{
	public class ProcedureLayoutsSelectionViewModel : SaveCancelDialogViewModel
	{
		public ObservableCollection<ProcedureLayoutItemViewModel> LayoutItems { get; private set; }
		public ProcedureLayoutsSelectionViewModel(List<Guid> procedureLayoutCollection)
		{
			Title = "Выбор макетов";
			LayoutItems = new ObservableCollection<ProcedureLayoutItemViewModel>();
			LayoutItems.Add(new ProcedureLayoutItemViewModel(ProcedureLayoutCollectionViewModel.NoLayout, procedureLayoutCollection.Contains(Guid.Empty)));
			foreach (var layoutItem in ClientManager.LayoutsConfiguration.Layouts.Select(x => new ProcedureLayoutItemViewModel(x, procedureLayoutCollection.Contains(x.UID))))
				LayoutItems.Add(layoutItem);
		}
	}

	public class ProcedureLayoutItemViewModel : BaseViewModel
	{
		public LayoutModel Layout { get; private set; }

		public ProcedureLayoutItemViewModel(LayoutModel layout, bool isChecked)
		{
			Layout = layout;
			IsChecked = isChecked;
		}

		public string Name
		{
			get { return Layout.Caption; }
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}