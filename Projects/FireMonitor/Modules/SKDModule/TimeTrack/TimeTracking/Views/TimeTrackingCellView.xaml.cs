﻿using System.Windows.Controls;
using System.Windows.Input;
using Infrastructure.Common.Windows;
using SKDModule.ViewModels;

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
				var timeTrackDetailsViewModel = new TimeTrackDetailsViewModel(dayTrackViewModel.DayTimeTrack, dayTrackViewModel.ShortEmployee);
				if (DialogService.ShowModalWindow(timeTrackDetailsViewModel))
				{
					if (!string.IsNullOrEmpty(dayTrackViewModel.DayTimeTrack.Error)) return;

					dayTrackViewModel.DayTimeTrack.Calculate();
					dayTrackViewModel.Update();
				}
			}
		}
	}
}