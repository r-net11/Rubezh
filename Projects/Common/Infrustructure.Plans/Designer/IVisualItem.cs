using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Infrustructure.Plans.Designer
{
	internal interface IVisualItem
	{
		bool IsEnabled { get; }
		bool IsBusy { get; }
		bool AllowDrag { get; }
		void SetIsMouseOver(bool isMouseOver, Point point);
		ContextMenu ContextMenuOpening();
		bool HitTest(Point point);

		void OnMouseDown(Point point, MouseButtonEventArgs e);
		void OnMouseUp(Point point, MouseButtonEventArgs e);
		void OnMouseMove(Point point, MouseEventArgs e);
		void OnMouseDoubleClick(Point point, MouseButtonEventArgs e);

		void DragStarted(Point point);
		void DragCompleted(Point point);
		void DragDelta(Point point, Vector shift);
	}
}
