using System.Windows.Controls;
using System.Windows.Input;
using Infrastructure.Common.Windows;
using SKDModule.Model;
using SKDModule.ViewModels;

namespace SKDModule.Views
{
	public partial class TimeTrackingDayControlView : UserControl
	{
		public TimeTrackingDayControlView()
		{
			InitializeComponent();
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var dayTrack = DataContext as DayTrack;
			if (dayTrack != null)
			{
				var timeTrackDetailsViewModel = new TimeTrackDetailsViewModel(dayTrack.DayTimeTrack, dayTrack.ShortEmployee);
				if (DialogService.ShowModalWindow(timeTrackDetailsViewModel))
				{
					if (!string.IsNullOrEmpty(dayTrack.DayTimeTrack.Error)) return;

					dayTrack.DayTimeTrack.Calculate();
					dayTrack.Update();
				}
			}
		}
	}
}