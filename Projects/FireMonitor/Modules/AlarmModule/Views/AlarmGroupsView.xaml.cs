using System.Windows.Controls;
using AlarmModule.ViewModels;

namespace AlarmModule.Views
{
	public partial class AlarmGroupsView : UserControl
	{
		public AlarmGroupsView()
		{
			InitializeComponent();
		}

        private void Button_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AlarmGroupsViewModel alarmGroupsViewModel = DataContext as AlarmGroupsViewModel;
            if (alarmGroupsViewModel != null)
            {
                alarmGroupsViewModel.ResetCommand.Execute();
            }
        }
	}
}