using System.Windows.Controls;
using System.Diagnostics;
using SKDModule.ViewModels;

namespace SKDModule.Views
{
	public partial class EmployeesView : UserControl
	{
		public EmployeesView()
		{
			InitializeComponent();
		}

		private void ItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			EmployeesViewModel.Current.DoNotEmployee = true;
		}

		private void Border_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			EmployeesViewModel.Current.SelectedEmployee = EmployeesViewModel.Current.SelectedEmployee;
		}
	}
}