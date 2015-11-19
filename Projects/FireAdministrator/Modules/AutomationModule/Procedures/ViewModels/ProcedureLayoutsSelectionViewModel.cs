using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;
using RubezhClient;
using System.Linq;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;

namespace AutomationModule.ViewModels
{
	public class ProcedureLayoutsSelectionViewModel : SaveCancelDialogViewModel
	{
		public ObservableCollection<ProcedureLayoutItemViewModel> LayoutItems { get; private set; }
		public ProcedureLayoutsSelectionViewModel(ProcedureLayoutCollection procedureLayoutCollection)
		{
			Title = "Выбор макетов";
			LayoutItems = new ObservableCollection<ProcedureLayoutItemViewModel>(ClientManager.LayoutsConfiguration.Layouts.Select(x => new ProcedureLayoutItemViewModel(x, procedureLayoutCollection.LayoutsUIDs.Contains(x.UID))));
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