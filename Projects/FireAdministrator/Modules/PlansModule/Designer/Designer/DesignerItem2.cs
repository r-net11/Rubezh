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

namespace PlansModule.Designer
{
	public class DesignerItem : ContentControl
	{
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set
			{
				SetValue(IsSelectedProperty, value);
				if (value)
					ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Publish(ElementBase.UID);
			}
		}

		public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));
		public bool IsSelectable
		{
			get { return (bool)GetValue(IsSelectableProperty); }
			set { SetValue(IsSelectableProperty, value); }
		}

		public static readonly DependencyProperty MoveThumbTemplateProperty = DependencyProperty.RegisterAttached("MoveThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));
		public static ControlTemplate GetMoveThumbTemplate(UIElement element)
		{
			return (ControlTemplate)element.GetValue(MoveThumbTemplateProperty);
		}
		public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
		{
			element.SetValue(MoveThumbTemplateProperty, value);
		}

		static DesignerItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
		}

		public DesignerCanvas DesignerCanvas
		{
			get { return VisualTreeHelper.GetParent(this) as DesignerCanvas; }
		}
		public DesignerItem(ElementBase element)
		{
			ElementBase = element;
			DataContext = element;
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Loaded += new RoutedEventHandler(this.DesignerItem_Loaded);
			MouseDoubleClick += (s, e) => OnShowProperties();
			IsVisibleLayout = true;
			IsSelectableLayout = true;
		}

		public IResizeChromeBase ResizeChromeBase { get; set; }
		public ElementBase ElementBase { get; private set; }

		bool _isVisibleLayout;
		public bool IsVisibleLayout
		{
			get { return _isVisibleLayout; }
			set
			{
				_isVisibleLayout = value;
				Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				if (!value)
					IsSelected = false;
			}
		}

		bool _isSelectableLayout;
		public bool IsSelectableLayout
		{
			get { return _isSelectableLayout; }
			set
			{
				_isSelectableLayout = value;
				IsSelectable = value;
				if (!value)
					IsSelected = false;
			}
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			//DesignerCanvas.BeginChange();
			//
		}
		public void ResetElement(ElementBase element)
		{
			ElementBase = element;
			Redraw();
		}
		public void Redraw()
		{
			//if (ElementBase is IElementZone)
			//{
			//    IElementZone elementZone = ElementBase as IElementZone;
			//    elementZone.Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo);
			//}

			//FrameworkElement frameworkElement = null;
			//if (ElementBase is ElementDevice)
			//{
			//    var elementDevice = ElementBase as ElementDevice;
			//    if (elementDevice.Device == null)
			//        elementDevice.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			//    if (elementDevice.Device != null)
			//    {
			//        frameworkElement = DeviceControl.GetDefaultPicture(elementDevice.Device.Driver.UID);
			//    }
			//    else
			//    {
			//        frameworkElement = ElementBase.Draw();
			//    }
			//}
			//else
			//{
			//    frameworkElement = ElementBase.Draw();
			//}

			//if (frameworkElement != null)
			//{
			//    frameworkElement.IsHitTestVisible = false;
			//    Content = frameworkElement;
			//}

			//Canvas.SetLeft(this, ElementBase.Left);
			//Canvas.SetTop(this, ElementBase.Top);
			//ItemWidth = ElementBase.Width;
			//ItemHeight = ElementBase.Height;

			//UpdateZoomDevice();
			//UpdatePolygonAdorner();
		}

		public void UpdatePolygonAdorner()
		{
			if (ResizeChromeBase != null)
				ResizeChromeBase.Initialize();
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
					DesignerCanvas.DeselectAll();
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
						ControlTemplate controlTemplate = DesignerItem.GetMoveThumbTemplate(contentVisual) as ControlTemplate;

						if (controlTemplate != null)
						{
							moveThumb.Template = controlTemplate;
						}
					}
				}
			}
			UpdateZoomDevice();
		}

		public void Remove()
		{
			//if (ElementBase is ElementDevice)
			//{
			//    var elementDevice = ElementBase as ElementDevice;
			//    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			//    device.PlanElementUIDs.Remove(elementDevice.UID);
			//    ServiceFactory.Events.GetEvent<DeviceRemovedEvent>().Publish(elementDevice.Device.UID);
			//}
		}

		public void SavePropertiesToElementBase()
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

		public void UpdateZoom()
		{
			if (ResizeChromeBase != null)
				ResizeChromeBase.UpdateZoom();
		}

		public void UpdateZoomDevice()
		{
			//if (IsDevice)
			//{
			//    this.Width = DesignerCanvas.PlanDesignerViewModel.DeviceZoom;
			//    this.Height = DesignerCanvas.PlanDesignerViewModel.DeviceZoom;
			//    Canvas.SetLeft(this, ElementBase.Left - this.Width / 2);
			//    Canvas.SetTop(this, ElementBase.Top - this.Height / 2);
			//}
		}

		public double ItemWidth
		{
			get { return Width; }
			set { Width = value; }
		}

		public double ItemHeight
		{
			get { return Height; }
			set { Height = value; }
		}
	}
}