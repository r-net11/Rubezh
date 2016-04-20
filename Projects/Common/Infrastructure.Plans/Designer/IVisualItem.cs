using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Infrastructure.Plans.Designer
{
	public interface IVisualItem
	{
		bool IsEnabled { get; }
		bool IsBusy { get; }
		bool AllowDrag { get; }
		void SetIsMouseOver(bool isMouseOver, Point point);
		ContextMenu ContextMenuOpening();
		IVisualItem HitTest(Point point);

		void OnMouseDown(Point point, MouseButtonEventArgs e);
		void OnMouseUp(Point point, MouseButtonEventArgs e);
		void OnMouseMove(Point point, MouseEventArgs e);
		void OnMouseDoubleClick(Point point, MouseButtonEventArgs e);

		void DragStarted(Point point);
		void DragCompleted(Point point);
		void DragDelta(Point point, Vector shift);
	}
}