using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Controls;
using Controls.TreeList;
using Infrastructure;
using Localization.SKD.Common;
using SKDModule.Events;
using SKDModule.ViewModels;

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
			_changeIsDeletedViewSubscriber = new ChangeIsDeletedViewSubscriber(this);
		}

		ChangeIsDeletedViewSubscriber _changeIsDeletedViewSubscriber;

		public TreeList TreeList
		{
			get { return _treeList; }
			set { _treeList = value; }
		}

		void OnUpdateAdditionalColumns(object obj)

		{
			UpdateAdditionalColumns();
			TreeList.RaiseEvent(new RoutedEventArgs(TreeList.LoadedEvent));
		}

		void treeList_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateAdditionalColumns();
			ChangeEmployeeGuest();
		}

		void UpdateAdditionalColumns()
		{
			var gridView = _treeList.View as GridView;
			var employeesViewModel = _treeList.DataContext as EmployeesViewModel;
			if (employeesViewModel != null && employeesViewModel.AdditionalColumnNames == null)
				return;

			const int columnCount = 2;

			if (gridView == null) return;

			for (var i = gridView.Columns.Count - 1; i >= columnCount; i--)
			{
				gridView.Columns.RemoveAt(i);
			}

			for (var i = 0; i < employeesViewModel.AdditionalColumnNames.Count; i++)
			{
				var gridViewColumn = new GridViewColumn {Header = employeesViewModel.AdditionalColumnNames[i], Width = 350};

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

		bool IsGuest { get { return (DataContext as EmployeesViewModel).PersonType == StrazhAPI.SKD.PersonType.Guest; } }
		GridView gridView { get { return (TreeList.View as GridView); } }
		GridViewColumn PositionColumn { get { return gridView.Columns.FirstOrDefault(x => x.Header == CommonResources.Position); } }
		GridViewColumn DescriptionColumn { get { return gridView.Columns.FirstOrDefault(x => x.Header == CommonResources.Note); } }
		bool IsPositionShown { get { return PositionColumn != null; } }
		bool IsDescriptionShown { get { return PositionColumn != null; } }

		void OnChangeEmployeeGuest(object obj)
		{
			ChangeEmployeeGuest();
			TreeList.RaiseEvent(new RoutedEventArgs(TreeList.LoadedEvent));
		}

		void ChangeEmployeeGuest()
		{
			if (IsGuest)
			{
				if (IsPositionShown)
				{
					gridView.Columns.Remove(PositionColumn);
				}
				if (!IsDescriptionShown)
				{
					var gridViewColumn = new GridViewColumn();
					gridViewColumn.Header = CommonResources.Note;
					gridViewColumn.Width = 150;

					var dataTemplate = new DataTemplate();
					var txtElement = new FrameworkElementFactory(typeof(IsDeletedTextBlock));
					dataTemplate.VisualTree = txtElement;
					var binding = new Binding();
					var bindingPath = string.Format("Description");
					binding.Path = new PropertyPath(bindingPath);
					binding.Mode = BindingMode.OneWay;
					txtElement.SetBinding(IsDeletedTextBlock.TextProperty, binding);
					SortBehavior.SetSortComparer(gridViewColumn, new EmployeeViewModelDescriptionComparer());
					gridViewColumn.CellTemplate = dataTemplate;
					gridView.Columns.Insert(2, gridViewColumn);
				}
			}
			else
			{
				if (IsDescriptionShown)
				{
					gridView.Columns.Remove(DescriptionColumn);
				}
				if (!IsPositionShown)
				{
					var gridViewColumn = new GridViewColumn();
					gridViewColumn.Header = CommonResources.Position;
					gridViewColumn.Width = 150;

					var dataTemplate = new DataTemplate();
					var txtElement = new FrameworkElementFactory(typeof(IsPositionDeletedTextBlock));
					dataTemplate.VisualTree = txtElement;
					var binding = new Binding();
					var bindingPath = string.Format("PositionName");
					binding.Path = new PropertyPath(bindingPath);
					binding.Mode = BindingMode.OneWay;
					txtElement.SetBinding(IsPositionDeletedTextBlock.TextProperty, binding);
					SortBehavior.SetSortComparer(gridViewColumn, new EmployeeViewModelPositionComparer());
					gridViewColumn.CellTemplate = dataTemplate;
					gridView.Columns.Insert(2, gridViewColumn);
				}
			}
		}
	}
}