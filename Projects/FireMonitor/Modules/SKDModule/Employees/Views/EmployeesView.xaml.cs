using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Controls.TreeList;
using FiresecAPI.SKD;
using Infrastructure;
using SKDModule.Events;
using SKDModule.ViewModels;
using Controls;

namespace SKDModule.Views
{
	public partial class EmployeesView : UserControl, IWithDeletedView
	{
		public EmployeesView()
		{
			InitializeComponent();
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Unsubscribe(OnUpdateAdditionalColumns);
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Subscribe(OnUpdateAdditionalColumns);
			ServiceFactory.Events.GetEvent<ChangeEmployeeGuestEvent>().Unsubscribe(OnChangeEmployeeGuest);
			ServiceFactory.Events.GetEvent<ChangeEmployeeGuestEvent>().Subscribe(OnChangeEmployeeGuest);
			ServiceFactory.Events.GetEvent<ChangeIsDeletedEvent>().Unsubscribe(OnChangeIsDeleted);
			ServiceFactory.Events.GetEvent<ChangeIsDeletedEvent>().Subscribe(OnChangeIsDeleted);
			//changeIsDeletedViewSubscriber = new ChangeIsDeletedViewSubscriber(this);
		}

		void OnUpdateAdditionalColumns(object obj)
		{
			treeList_Loaded(_treeList, null);
		}

		public TreeList TreeList
		{
			get { return _treeList; }
			set { _treeList = value; }
		}

		ChangeIsDeletedViewSubscriber changeIsDeletedViewSubscriber;

		void OnChangeEmployeeGuest(object obj)
		{
			GridView gridView = _treeList.View as GridView;
			EmployeesViewModel employeesViewModel = _treeList.DataContext as EmployeesViewModel;
			if (employeesViewModel.PersonType == FiresecAPI.SKD.PersonType.Employee)
			{
				var gridViewColumn = new GridViewColumn();
				gridViewColumn.Header = "Должность";
				gridViewColumn.Width = 150;

				var dataTemplate = new DataTemplate();
				var txtElement = new FrameworkElementFactory(typeof(TextBlock));
				dataTemplate.VisualTree = txtElement;
				var binding = new Binding();
				var bindingPath = string.Format("PositionName");
				binding.Path = new PropertyPath(bindingPath);
				binding.Mode = BindingMode.OneWay;
				txtElement.SetBinding(TextBlock.TextProperty, binding);

				gridViewColumn.CellTemplate = dataTemplate;
				gridView.Columns.Add(gridViewColumn);

			}
			else
			{
				var gridViewColumn = gridView.Columns.FirstOrDefault(x => x.Header == "Должность");
				if (gridViewColumn != null)
				{
					gridView.Columns.Remove(gridViewColumn);
				}
			}
		}

		void OnChangeIsDeleted(LogicalDeletationType deletationType)
		{
			GridView gridView = _treeList.View as GridView;
			EmployeesViewModel employeesViewModel = _treeList.DataContext as EmployeesViewModel;
			if (deletationType == LogicalDeletationType.All)
			{
				var gridViewColumn = new GridViewColumn();
				gridViewColumn.Header = "Дата удаления";
				gridViewColumn.Width = 150;

				var dataTemplate = new DataTemplate();
				var txtElement = new FrameworkElementFactory(typeof(TextBlock));
				dataTemplate.VisualTree = txtElement;
				var binding = new Binding();
				var bindingPath = string.Format("RemovalDate");
				binding.Path = new PropertyPath(bindingPath);
				binding.Mode = BindingMode.OneWay;
				txtElement.SetBinding(TextBlock.TextProperty, binding);
				gridViewColumn.CellTemplate = dataTemplate;
				gridView.Columns.Add(gridViewColumn);
			}
			else
			{
				var gridViewColumn = gridView.Columns.FirstOrDefault(x => x.Header == "Дата удаления");
				if (gridViewColumn != null)
				{
					gridView.Columns.Remove(gridViewColumn);
				}
			}
		}

		private void treeList_Loaded(object sender, RoutedEventArgs e)
		{
			GridView gridView = _treeList.View as GridView;
			EmployeesViewModel employeesViewModel = _treeList.DataContext as EmployeesViewModel;
			if (employeesViewModel.AdditionalColumnNames == null)
				return;

			for (int i = gridView.Columns.Count - 1; i >= 4; i--)
				gridView.Columns.RemoveAt(i);

			for (int i = 0; i < employeesViewModel.AdditionalColumnNames.Count; i++)
			{
				var gridViewColumn = new GridViewColumn();
				gridViewColumn.Header = employeesViewModel.AdditionalColumnNames[i];
				gridViewColumn.Width = 150;

				var dataTemplate = new DataTemplate();
				var txtElement = new FrameworkElementFactory(typeof(TextBlock));
				dataTemplate.VisualTree = txtElement;
				var binding = new Binding();
				var bindingPath = string.Format("AdditionalColumnValues[{0}]", i);
				binding.Path = new PropertyPath(bindingPath);
				binding.Mode = BindingMode.OneWay;
				txtElement.SetBinding(TextBlock.TextProperty, binding);

				gridViewColumn.CellTemplate = dataTemplate;
				gridView.Columns.Add(gridViewColumn);
			}
		}
	}

	public interface IWithDeletedView
	{
		TreeList TreeList { get; set; }
	}

	public class ChangeIsDeletedViewSubscriber
	{
		IWithDeletedView _parent;

		public ChangeIsDeletedViewSubscriber(IWithDeletedView parent)
		{
			_parent = parent;
			ServiceFactory.Events.GetEvent<ChangeIsDeletedEvent>().Unsubscribe(OnChangeIsDeleted);
			ServiceFactory.Events.GetEvent<ChangeIsDeletedEvent>().Subscribe(OnChangeIsDeleted);
		}

		void OnChangeIsDeleted(LogicalDeletationType deletationType)
		{
			GridView gridView = _parent.TreeList.View as GridView;
			EmployeesViewModel employeesViewModel = _parent.TreeList.DataContext as EmployeesViewModel;
			if (deletationType == LogicalDeletationType.All)
			{
				var gridViewColumn = new GridViewColumn();
				gridViewColumn.Header = "Дата удаления";
				gridViewColumn.Width = 150;

				var dataTemplate = new DataTemplate();
				var txtElement = new FrameworkElementFactory(typeof(TextBlock));
				dataTemplate.VisualTree = txtElement;
				var binding = new Binding();
				var bindingPath = string.Format("RemovalDate");
				binding.Path = new PropertyPath(bindingPath);
				binding.Mode = BindingMode.OneWay;
				txtElement.SetBinding(TextBlock.TextProperty, binding);

				gridViewColumn.CellTemplate = dataTemplate;
				gridView.Columns.Add(gridViewColumn);
			}
			else
			{
				var gridViewColumn = gridView.Columns.FirstOrDefault(x => x.Header == "Дата удаления");
				if (gridViewColumn != null)
				{
					gridView.Columns.Remove(gridViewColumn);
				}
			}
		}
	}
}