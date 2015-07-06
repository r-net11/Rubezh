using Infrastructure.Common.TreeList;
using System.Windows;

namespace Infrastructure.Common.Services.DragDrop
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