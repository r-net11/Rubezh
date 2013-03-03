using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrastructure.Common.Services.DragDrop
{
	public interface IDragDropService
	{
		void DoDragDrop(IDataObject dataObject, UIElement dragSource);
		void DoDragDrop(IDataObject dataObject, UIElement dragSource, DragDropEffects effects);
		void DoDragDrop(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush);
		void DoDragDrop(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush, DragDropEffects effects);
	}
}
