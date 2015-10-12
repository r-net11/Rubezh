using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Controls;
using Controls.TreeList;
using RubezhAPI.SKD;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule
{
	public class ChangeIsDeletedViewSubscriber
	{
		IWithDeletedView _parent;
		LogicalDeletationType _deletationType;
		GridView gridView { get { return (_parent.TreeList.View as GridView); } }
		GridViewColumn IsDeletedColumn { get { return gridView.Columns.FirstOrDefault(x => x.Header == "Дата удаления"); } }
		bool IsColumnShown { get { return IsDeletedColumn != null; } }

		public ChangeIsDeletedViewSubscriber(IWithDeletedView parent, LogicalDeletationType deletationType = LogicalDeletationType.Active)
		{
			_parent = parent;
			_deletationType = deletationType;
			ChangeIsDeleted();
			_parent.TreeList.Loaded += new RoutedEventHandler(TreeList_Loaded);
			ServiceFactory.Events.GetEvent<ChangeIsDeletedEvent>().Unsubscribe(OnChangeIsDeleted);
			ServiceFactory.Events.GetEvent<ChangeIsDeletedEvent>().Subscribe(OnChangeIsDeleted);
		}
		 
		void OnChangeIsDeleted(LogicalDeletationType deletationType)
		{
			_deletationType = deletationType;
			ChangeIsDeleted();
			_parent.TreeList.RaiseEvent(new RoutedEventArgs(TreeList.LoadedEvent));
		}

		public void TreeList_Loaded(object sender, RoutedEventArgs e)
		{
			ChangeIsDeleted();
		}

		void ChangeIsDeleted()
		{
			if (_deletationType == LogicalDeletationType.All)
			{
				if (!IsColumnShown)
				{
					var gridViewColumn = new GridViewColumn();
					gridViewColumn.Header = "Дата удаления";
					gridViewColumn.Width = 150;
					var dataTemplate = new DataTemplate();
					var txtElement = new FrameworkElementFactory(typeof(IsDeletedTextBlock));
					dataTemplate.VisualTree = txtElement;
					var binding = new Binding();
					var bindingPath = string.Format("RemovalDate");
					binding.Path = new PropertyPath(bindingPath);
					binding.Mode = BindingMode.OneWay;
					txtElement.SetBinding(IsDeletedTextBlock.TextProperty, binding);
					gridViewColumn.CellTemplate = dataTemplate;
					ListViewLayoutManager.SetCanUserResize(gridViewColumn, false);
					gridView.Columns.Add(gridViewColumn);
				}
			}
			else if (IsColumnShown)
			{
				gridView.Columns.Remove(IsDeletedColumn);
			}
		}
	}

	public interface IWithDeletedView
	{
		TreeList TreeList { get; set; }
	}

}
