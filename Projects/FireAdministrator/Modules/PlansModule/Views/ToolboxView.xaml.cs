using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;
using PlansModule.ViewModels;
using Infrustructure.Plans.Elements;

namespace PlansModule.Views
{
	public partial class ToolboxView : UserControl
	{
		public ToolboxView()
		{
			InitializeComponent();
		}

		private Point? dragStartPoint = null;
		private void On_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			this.dragStartPoint = new Point?(e.GetPosition(this));
			e.Handled = true;
		}
		private void On_MouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				this.dragStartPoint = null;
			}

			if (this.dragStartPoint.HasValue)
			{
				var elementBase = CreateNewElementBase((sender as Controls.ToolBarButton).Name);
				var dataObject = new DataObject("DESIGNER_ITEM", elementBase);
				DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
				e.Handled = true;
			}

			e.Handled = true;
		}
		ElementBase CreateNewElementBase(string itemType)
		{
			ElementBase elementBase = null;

			switch (itemType)
			{
				case "ElementPolyLine":
					elementBase = new ElementPolyline();
					break;

				case "ElementRectangle":
					elementBase = new ElementRectangle();
					break;

				case "ElementEllipse":
					elementBase = new ElementEllipse();
					break;

				case "ElementPolygon":
					elementBase = new ElementPolygon();
					break;

				case "ElementTextBlock":
					elementBase = new ElementTextBlock();
					break;

				case "ElementRectangleZone":
					elementBase = new ElementRectangleZone();
					break;

				case "ElementPolygonZone":
					elementBase = new ElementPolygonZone();
					break;

				case "ElementSubPlan":
					elementBase = new ElementSubPlan();
					break;
			}

			return elementBase;
		}
	}
}