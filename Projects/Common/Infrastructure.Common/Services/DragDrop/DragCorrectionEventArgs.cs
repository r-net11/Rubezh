using System;
using System.Windows;
using System.Windows.Input;

namespace Infrastructure.Common.Services.DragDrop
{
	public delegate void DragCorrectionEventHandler(object sender, DragCorrectionEventArgs e);
	public class DragCorrectionEventArgs : EventArgs
	{
		public IDataObject Data { get; private set; }
		public Vector Correction { get; set; }

		private Converter<IInputElement, Point> _getPosition;

		public DragCorrectionEventArgs(IDataObject data, MouseEventArgs me)
		{
			Data = data;
			_getPosition = me.GetPosition;
			Correction = new Vector(0, 0);
		}
		public DragCorrectionEventArgs(DragEventArgs de)
		{
			Data = de.Data;
			_getPosition = de.GetPosition;
			Correction = new Vector(0, 0);
		}

		public Point GetPosition(IInputElement relativeTo)
		{
			return _getPosition(relativeTo);
		}
	}
}
