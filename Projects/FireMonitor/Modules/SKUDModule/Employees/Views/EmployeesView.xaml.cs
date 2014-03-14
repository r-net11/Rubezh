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
			var employeesViewModel = DataContext as EmployeesViewModel;
			if (employeesViewModel != null && employeesViewModel.SelectedOrganisationEmployee != null)
			{
				employeesViewModel.SelectedOrganisationEmployee.DoNotSelectEmployee = true;
			}
		}

		private void Border_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var employeesViewModel = DataContext as EmployeesViewModel;
			if (employeesViewModel != null && employeesViewModel.SelectedOrganisationEmployee != null)
			{
				employeesViewModel.SelectedOrganisationEmployee.DoNotSelectEmployee = false;
				employeesViewModel.SelectedOrganisationEmployee.SelectedEmployee = employeesViewModel.SelectedOrganisationEmployee.SelectedEmployee;
			}
		}
	}
}