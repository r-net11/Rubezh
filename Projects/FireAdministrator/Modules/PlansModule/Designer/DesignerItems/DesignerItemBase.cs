using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure.Common;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemBase : DesignerItem
	{
		static DesignerItemBase()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
		}

		public DesignerItemBase(ElementBase element)
			: base(element)
		{
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			MouseDoubleClick += (s, e) => OnShowProperties();
			IsVisibleLayout = true;
			IsSelectableLayout = true;
		}


		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			//DesignerCanvas.BeginChange();
			//
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);

			if (DesignerCanvas != null)
			{
				if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
					IsSelected = !IsSelected;
				else if (!IsSelected)
				{
					((DesignerCanvas)DesignerCanvas).DeselectAll();
					IsSelected = true;
				}
			}
			e.Handled = false;
		}

		public override void Remove()
		{
			//if (ElementBase is ElementDevice)
			//{
			//    var elementDevice = ElementBase as ElementDevice;
			//    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			//    device.PlanElementUIDs.Remove(elementDevice.UID);
			//    ServiceFactory.Events.GetEvent<DeviceRemovedEvent>().Publish(elementDevice.Device.UID);
			//}
		}
		public override void UpdateElementProperties()
		{
			//if (ElementBase is ElementBasePolygon)
			//{
			//    ElementBasePolygon elementPolygon = ElementBase as ElementBasePolygon;
			//    elementPolygon.PolygonPoints = new PointCollection();
			//    if (this.Content != null)
			//        foreach (var point in (this.Content as Polygon).Points)
			//        {
			//            elementPolygon.PolygonPoints.Add(new Point(point.X, point.Y));
			//        }
			//}
			//if (ElementBase is ElementPolyline)
			//{
			//    ElementPolyline elementPolyline = ElementBase as ElementPolyline;
			//    elementPolyline.PolygonPoints = new PointCollection();
			//    if (this.Content != null)
			//        foreach (var point in (this.Content as Polyline).Points)
			//        {
			//            elementPolyline.PolygonPoints.Add(new Point(point.X, point.Y));
			//        }
			//}
			//if (ElementBase is ElementDevice)
			//{
			//    ElementBase.Left = Canvas.GetLeft(this) + this.Width / 2;
			//    ElementBase.Top = Canvas.GetTop(this) + this.Height / 2;
			//    ElementBase.Width = 0;
			//    ElementBase.Height = 0;
			//}
		}
	}
}