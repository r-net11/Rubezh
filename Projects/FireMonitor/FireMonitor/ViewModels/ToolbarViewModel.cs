using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.ViewModels
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

			//<StackPanel Grid.Row="1" Orientation="Horizontal" VerticalAlignment="Center">
			//    <Views:SoundView x:Name="soundView" Margin="2" />
			//    <Views:ConnectionIndicatorView Margin="2" />
			//    <Views:GKConnectionIndicator Margin="2" />
			//    <Views:UserView Margin="2" />
			//    <Views:AutoActivationView Margin="2" />
			//</StackPanel>
	}
	}
}
