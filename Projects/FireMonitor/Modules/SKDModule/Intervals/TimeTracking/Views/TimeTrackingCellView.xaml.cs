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
			EmployeeTimeTrack employeeTimeTrack = DataContext as EmployeeTimeTrack;
			if (employeeTimeTrack != null)
			{
				var timeTrackDetailsViewModel = new TimeTrackDetailsViewModel(employeeTimeTrack);
				DialogService.ShowModalWindow(timeTrackDetailsViewModel);
			}
		}
	}
}