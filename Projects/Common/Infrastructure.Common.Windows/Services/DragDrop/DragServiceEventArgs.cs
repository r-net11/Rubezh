using System;
using System.Windows;

namespace Infrastructure.Common.Windows.Services.DragDrop
{
	public delegate void DragServiceEventHandler(object sender, DragServiceEventArgs e);
	public class DragServiceEventArgs : EventArgs
	{
		public IDataObject Data { get; private set; }
		public DragDropEffects Effects { get; set; }
		public bool Handled { get; set; }

		public DragServiceEventArgs(IDataObject data)
		{
			Handled = false;
			Data = data;
			Effects = DragDropEffects.None;
		}
	}
}
