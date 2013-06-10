using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Infrastructure.Common.TreeList;

namespace Controls.TreeList
{
	public class TreeListItem : ListViewItem, INotifyPropertyChanged
	{
		private TreeNodeViewModel _node;
		public TreeNodeViewModel Node
		{
			get { return _node; }
			internal set
			{
				_node = value;
				OnPropertyChanged("Node");
			}
		}

		public TreeList Tree { get; internal set; }

		public TreeListItem()
		{
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.Source.GetType() != typeof(RowExpander) && Node != null && Node.IsExpandable)
				Node.IsExpanded = !Node.IsExpanded;
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (Node != null)
				switch (e.Key)
				{
					case Key.Right:
						e.Handled = true;
						if (!Node.IsExpanded)
						{
							Node.IsExpanded = true;
							ChangeFocus(Node);
						}
						else if (Node.Nodes.Count > 0)
							ChangeFocus(Node.Nodes[0]);
						break;
					case Key.Left:
						e.Handled = true;
						if (Node.IsExpanded && Node.IsExpandable)
						{
							Node.IsExpanded = false;
							ChangeFocus(Node);
						}
						else
							ChangeFocus(Node.ParentNode);
						break;
					case Key.Subtract:
						e.Handled = true;
						Node.IsExpanded = false;
						ChangeFocus(Node);
						break;
					case Key.Add:
						e.Handled = true;
						Node.IsExpanded = true;
						ChangeFocus(Node);
						break;
				}
			if (!e.Handled)
				base.OnKeyDown(e);
		}

		private void ChangeFocus(TreeNodeViewModel node)
		{
			if (Tree != null)
			{
				var item = Tree.ItemContainerGenerator.ContainerFromItem(node) as TreeListItem;
				if (item != null)
					item.Focus();
				else
					Tree.PendingFocusNode = node;
			}
		}

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#endregion
	}
}