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
			treeList_Loaded(_treeList, null);
		}

		private void treeList_Loaded(object sender, RoutedEventArgs e)
		{
			GridView gridView = _treeList.View as GridView;
			EmployeesViewModel employeesViewModel = _treeList.DataContext as EmployeesViewModel;

			for (int i = 0; i < employeesViewModel.AdditionalColumnNames.Count; i++)
			{
				var gridViewColumn = new GridViewColumn();
				gridViewColumn.Header = employeesViewModel.AdditionalColumnNames[i];

				var dataTemplate = new DataTemplate();
				var txtElement = new FrameworkElementFactory(typeof(TextBlock));
				dataTemplate.VisualTree = txtElement;
				var binding = new Binding();
				var bindingPath = string.Format("AdditionalColumnValues[{0}]", 0);
				binding.Path = new PropertyPath(bindingPath);
				binding.Mode = BindingMode.OneWay;
				txtElement.SetBinding(TextBlock.TextProperty, binding);

				gridViewColumn.CellTemplate = dataTemplate;
				gridView.Columns.Add(gridViewColumn);
			}
		}
	}
}