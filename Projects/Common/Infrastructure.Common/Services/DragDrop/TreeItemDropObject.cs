using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrastructure.Common.Services.DragDrop
{
	public class TreeItemDropObject
	{
		public IDataObject DataObject { get; internal set; }
		public TreeItemViewModel Target { get; internal set; }

		public TreeItemDropObject()
		{
		}
	}
}
