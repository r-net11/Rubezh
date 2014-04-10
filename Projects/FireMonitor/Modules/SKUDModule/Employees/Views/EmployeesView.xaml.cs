using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Infrastructure;
using SKDModule.Events;
using SKDModule.ViewModels;

namespace SKDModule.Views
{
	public partial class EmployeesView : UserControl
	{
		public EmployeesView()
		{
			InitializeComponent();
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Unsubscribe(OnUpdateAdditionalColumns);
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Subscribe(OnUpdateAdditionalColumns);
		}

		void OnUpdateAdditionalColumns(object obj)
		{
			_dataGrid_Loaded(_dataGrid, null);
		}

		private void _dataGrid_Loaded(object sender, RoutedEventArgs e)
		{
			EmployeesViewModel employeesViewModel = _dataGrid.DataContext as EmployeesViewModel;
			if (employeesViewModel != null)
			{
				var columns = _dataGrid.Columns.Where(x => x.Header.ToString() != "Имя" && x.Header.ToString() != "Фамилия" && x.Header.ToString() != "Отчество").ToList();
				foreach (var column in columns)
				{
					_dataGrid.Columns.Remove(column);
				}

				for (int i = 0; i < employeesViewModel.AdditionalColumnNames.Count; i++)
				{
					var additionalColumnName = employeesViewModel.AdditionalColumnNames[i];
					DataGridTextColumn textColumn = new DataGridTextColumn();
					textColumn.Header = additionalColumnName;
					textColumn.Binding = new Binding(string.Format("AdditionalColumnValues[{0}]", i));
					_dataGrid.Columns.Add(textColumn);
				}
			}
		}
	}
}