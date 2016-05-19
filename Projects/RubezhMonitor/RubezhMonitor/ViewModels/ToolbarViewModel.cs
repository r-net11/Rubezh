using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;

namespace RubezhMonitor.ViewModels
{
	public class ToolbarViewModel : BaseViewModel
	{
		private BaseViewModel _alarmGroups;
		public BaseViewModel AlarmGroups
		{
			get { return _alarmGroups; }
			set
			{
				_alarmGroups = value;
				OnPropertyChanged("AlarmGroups");
			}
		}

		private ObservableCollection<BaseViewModel> _toolbarItems;
		public ObservableCollection<BaseViewModel> ToolbarItems
		{
			get { return _toolbarItems; }
			set
			{
				_toolbarItems = value;
				OnPropertyChanged("ToolbarItems");
			}
		}

		public ToolbarViewModel()
		{
			ToolbarItems = new ObservableCollection<BaseViewModel>();
		}
	}
}