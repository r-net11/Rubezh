using System;
using System.Windows;

namespace Infrastructure.Common.Windows.Services.DragDrop
{
	public interface IDragDropService
	{
		bool IsDragging { get; }
		void DoDragDrop(IDataObject dataObject, UIElement dragSource);
		void DoDragDrop(IDataObject dataObject, UIElement dragSource, DragDropEffects effects);
		void DoDragDrop(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush);
		void DoDragDrop(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush, DragDropEffects effects);
		void DoDragDropSimulate(IDataObject dataObject, UIElement dragSource, Action callback);
		void DoDragDropSimulate(IDataObject dataObject, UIElement dragSource, DragDropEffects effects, Action callback);
		void DoDragDropSimulate(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush, Action callback);
		void DoDragDropSimulate(IDataObject dataObject, UIElement dragSource, bool showDragVisual, bool useVisualBrush, DragDropEffects effects, Action callback);
		void StopDragSimulate(bool cancel = true);
		event DragServiceEventHandler DragOver;
		event DragServiceEventHandler Drop;
		event DragCorrectionEventHandler DragCorrection;
	}
}
