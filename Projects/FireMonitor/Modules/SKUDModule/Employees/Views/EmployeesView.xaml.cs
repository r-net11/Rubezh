using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SKDModule.ViewModels;

namespace SKDModule.Views
{
	public partial class EmployeesView : UserControl
	{
		public EmployeesView()
		{
			InitializeComponent();
		}

		private void _dataGrid_Loaded(object sender, RoutedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			OrganisationEmployeesViewModel organisationEmployeesViewModel = dataGrid.DataContext as OrganisationEmployeesViewModel;
			if (organisationEmployeesViewModel != null)
			{
				for (int i = 0; i < organisationEmployeesViewModel.AdditionalColumnNames.Count; i++)
				{
					var additionalColumnName = organisationEmployeesViewModel.AdditionalColumnNames[i];
					DataGridTextColumn textColumn = new DataGridTextColumn();
					textColumn.Header = additionalColumnName;
					textColumn.Binding = new Binding(string.Format("AdditionalColumnValues[{0}]", i));
					dataGrid.Columns.Add(textColumn);
				}
			}
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