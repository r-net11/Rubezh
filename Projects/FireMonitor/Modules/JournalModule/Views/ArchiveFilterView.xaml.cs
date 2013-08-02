using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JournalModule.ViewModels;

namespace JournalModule.Views
{
	public partial class ArchiveFilterView : UserControl
	{
		public ArchiveFilterView()
		{
			InitializeComponent();
		}

		void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			var checkBox = sender as CheckBox;
			if (checkBox.IsChecked != null)
			{
				bool isChecked = (bool)checkBox.IsChecked;
				var archiveFilter = DataContext as ArchiveFilterViewModel;
				var stateType = (checkBox.DataContext as ClassViewModel).StateType;
				archiveFilter.JournalEvents.Where(x => x.StateType == stateType).All(x => (x.IsEnable = isChecked) == isChecked);
			}
		}

		void CheckBox_Checked_1(object sender, RoutedEventArgs e)
		{
			var checkBox = sender as CheckBox;
			var archiveFilter = DataContext as ArchiveFilterViewModel;
			var stateType = (checkBox.DataContext as EventViewModel).StateType;

			if (archiveFilter.JournalEvents.Where(x => x.StateType == stateType).All(x => x.IsEnable == checkBox.IsChecked))
				archiveFilter.JournalTypes.Find(x => x.StateType == stateType).IsEnable = checkBox.IsChecked;
			else
				archiveFilter.JournalTypes.Find(x => x.StateType == stateType).IsEnable = null;
		}

		void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
		{
			var checkBox = sender as CheckBox;
			var archiveFilter = DataContext as ArchiveFilterViewModel;
			var stateType = (checkBox.DataContext as ClassViewModel).StateType;
			if (archiveFilter.JournalEvents.Where(x => x.StateType == stateType).All(x => x.IsEnable))
			{
				checkBox.IsChecked = false;
				e.Handled = true;
			}
			else if (archiveFilter.JournalEvents.Where(x => x.StateType == stateType).All(x => !x.IsEnable))
			{
				checkBox.IsChecked = true;
				e.Handled = true;
			}
		}
	}
}