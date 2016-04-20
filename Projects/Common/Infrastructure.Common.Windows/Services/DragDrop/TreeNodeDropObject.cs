using System.Windows;
using Infrastructure.Common.Windows.TreeList;

namespace Infrastructure.Common.Windows.Services.DragDrop
{
	public class TreeNodeDropObject
	{
		public IDataObject DataObject { get; internal set; }
		public TreeNodeViewModel Target { get; internal set; }

		public TreeNodeDropObject()
		{
		}
	}
}
