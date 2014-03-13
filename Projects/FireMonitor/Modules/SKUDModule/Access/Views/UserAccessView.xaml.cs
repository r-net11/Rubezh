using System.Windows.Controls;
using System.Diagnostics;
using SKDModule.ViewModels;

namespace SKDModule.Views
{
	public partial class UsersAccessView : UserControl
	{
		public UsersAccessView()
		{
			InitializeComponent();
		}

		private void ItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			UsersAccessViewModel.Current.ResetUser = true;
		}

		private void Border_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			UsersAccessViewModel.Current.SelectedUser = UsersAccessViewModel.Current.RealSelectedUser;
		}
	}
}