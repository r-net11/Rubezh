using System.Windows.Controls;
using System.Windows.Input;
using SKDModule.ViewModels;
using Infrastructure.Common.Windows;
using FiresecAPI.SKD;

namespace SKDModule.Views
{
	public partial class TimeTrackingCellView : UserControl
	{
		public TimeTrackingCellView()
		{
			InitializeComponent();
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			DayTrackViewModel dayTrackViewModel = DataContext as DayTrackViewModel;
			if (dayTrackViewModel != null)
			{
				var timeTrackDetailsViewModel = new TimeTrackDetailsViewModel(dayTrackViewModel.EmployeeTimeTrack);
				DialogService.ShowModalWindow(timeTrackDetailsViewModel);
			}
		}
	}
}