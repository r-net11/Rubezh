using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DeviceControls;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using PlansModule.Events;
using PlansModule.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemBase : DesignerItem
	{
		//public static readonly DependencyProperty MoveThumbTemplateProperty = DependencyProperty.RegisterAttached("MoveThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));
		//public static ControlTemplate GetMoveThumbTemplate(UIElement element)
		//{
		//    return (ControlTemplate)element.GetValue(MoveThumbTemplateProperty);
		//}
		//public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
		//{
		//    element.SetValue(MoveThumbTemplateProperty, value);
		//}

		static DesignerItemBase()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
		}

		public DesignerItemBase(ElementBase element)
			: base(element)
		{
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Loaded += new RoutedEventHandler(this.DesignerItem_Loaded);
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

		public override void Redraw()
		{
			//    if (ElementBase is IElementZone)
			//    {
			//        IElementZone elementZone = ElementBase as IElementZone;
			//        elementZone.Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo);
			//    }

			//    FrameworkElement frameworkElement = null;
			//    if (ElementBase is ElementDevice)
			//    {
			//        var elementDevice = ElementBase as ElementDevice;
			//        if (elementDevice.Device == null)
			//            elementDevice.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			//        if (elementDevice.Device != null)
			//        {
			//            frameworkElement = DeviceControl.GetDefaultPicture(elementDevice.Device.Driver.UID);
			//        }
			//        else
			//        {
			//            frameworkElement = ElementBase.Draw();
			//        }
			//    }
			//    else
			//    {
			//        frameworkElement = ElementBase.Draw();
			//    }

			FrameworkElement frameworkElement = Element.Draw();
			if (frameworkElement != null)
			{
				frameworkElement.IsHitTestVisible = false;
				Content = frameworkElement;
			}
			SetLocation(Element.GetRectangle());

			//UpdateZoomDevice();
			UpdateAdorner();
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

		private void DesignerItem_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.Template != null)
			{
				ContentPresenter contentPresenter = this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
				MoveThumb moveThumb = this.Template.FindName("PART_MoveThumbRectangle", this) as MoveThumb;

				if (contentPresenter != null && moveThumb != null)
				{
					UIElement contentVisual = null;
					if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
					{
						contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
					}

					if (contentVisual != null)
					{
						//ControlTemplate controlTemplate = DesignerItemBase.GetMoveThumbTemplate(contentVisual) as ControlTemplate;

						//if (controlTemplate != null)
						//{
						//    moveThumb.Template = controlTemplate;
						//}
					}
				}
			}
			UpdateZoomDevice();
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
			//ElementBase.Left = Canvas.GetLeft(this);
			//ElementBase.Top = Canvas.GetTop(this);
			//ElementBase.Width = this.ItemWidth;
			//ElementBase.Height = this.ItemHeight;

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

		public override double ItemWidth
		{
			get { return Width; }
			set { Width = value; }
		}
		public override double ItemHeight
		{
			get { return Height; }
			set { Height = value; }
		}
	}
}