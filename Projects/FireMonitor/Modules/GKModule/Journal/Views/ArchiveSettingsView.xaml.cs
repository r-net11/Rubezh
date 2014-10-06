using System.Windows;
using System.Windows.Controls;
using GKModule.ViewModels;
using Infrastructure.Models;

namespace GKModule.Views
{
	public partial class ArchiveSettingsView : UserControl
	{
		public ArchiveSettingsView()
		{
			InitializeComponent();
		}

		void _archiveDefaultStateTypes_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			OnCheckedArchiveDefaultStateChanged();
		}

		void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			OnCheckedArchiveDefaultStateChanged();
		}

		void OnCheckedArchiveDefaultStateChanged()
		{
			switch ((DataContext as ArchiveSettingsViewModel).CheckedArchiveDefaultStateType)
			{
				case ArchiveDefaultStateType.LastHours:
					_countHoursPanel.Visibility = Visibility.Visible;
					_countDaysPanel.Visibility = Visibility.Collapsed;
					_startDatePanel.Visibility = Visibility.Collapsed;
					_endDatePanel.Visibility = Visibility.Collapsed;
					break;

				case ArchiveDefaultStateType.LastDays:
					_countHoursPanel.Visibility = Visibility.Collapsed;
					_countDaysPanel.Visibility = Visibility.Visible;
					_startDatePanel.Visibility = Visibility.Collapsed;
					_endDatePanel.Visibility = Visibility.Collapsed;
					break;

				case ArchiveDefaultStateType.FromDate:
					_countHoursPanel.Visibility = Visibility.Collapsed;
					_countDaysPanel.Visibility = Visibility.Collapsed;
					_startDatePanel.Visibility = Visibility.Visible;
					_endDatePanel.Visibility = Visibility.Collapsed;
					break;

				case ArchiveDefaultStateType.RangeDate:
					_countHoursPanel.Visibility = Visibility.Collapsed;
					_countDaysPanel.Visibility = Visibility.Collapsed;
					_startDatePanel.Visibility = Visibility.Visible;
					_endDatePanel.Visibility = Visibility.Visible;
					break;

				default:
					break;
			}
		}
	}
}