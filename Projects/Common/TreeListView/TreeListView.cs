using System.Windows;
using System.Windows.Controls;

namespace Ricciolo.Controls
{
	public class TreeListView : TreeView
	{
		static TreeListView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));

			//ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
			//template.Seal();
			//ItemsControl.ItemsPanelProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(template));
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TreeListViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is TreeListViewItem;
		}

		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
		}

		public GridViewColumnCollection Columns
		{
			get
			{
				if (_columns == null)
				{
					_columns = new GridViewColumnCollection();
				}

				return _columns;
			}
		}

		private GridViewColumnCollection _columns;
	}
}