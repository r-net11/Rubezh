using Infrastructure.Common.Windows;
using System.Windows.Forms;
namespace ManagementConsole
{
	public partial class ManagementConsoleView
	{
		ManagementConsoleViewModel ManagementConsoleViewModel;

		public ManagementConsoleView()
		{
			InitializeComponent();
			ManagementConsoleViewModel = new ManagementConsoleViewModel();
			DataContext = ManagementConsoleViewModel;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (ManagementConsoleViewModel.HasChanges)
			{
				if (MessageBox.Show("Сохранить изменения?", "Сохранить изменения?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
				{
					ManagementConsoleViewModel.GlobalSettingsViewModel.SaveCommand.Execute();
				}
			}
		}
	}
}