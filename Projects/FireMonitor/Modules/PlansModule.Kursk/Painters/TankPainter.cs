using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Controls.Converters;
using System;

namespace PlansModule.Kursk.Designer
{
	class TankPainter : RectangleZonePainter
	{
		private PresenterItem _presenterItem;
		private XDevice _device;
		private ContextMenu _contextMenu;
		private static Brush _brush;

		static TankPainter()
		{
			_brush = new ImageBrush()
			{
				ImageSource = new BitmapImage(ResourceHelper.ComposeResourceUri(typeof(BoolToVisibilityConverter).Assembly, "Images/BArrowUpDown.png")),
				Stretch = Stretch.Fill,
			};
			_brush.Freeze();
		}
		public TankPainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			var elementRectangleTank = presenterItem.Element as ElementRectangleTank;
			if (elementRectangleTank != null)
			{
				_device = Helper.GetXDevice(elementRectangleTank);
				if (_device != null && _device.DeviceState != null)
					_device.DeviceState.StateChanged += OnPropertyChanged;
			}

			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			//_presenterItem.Cursor = Cursors.Hand;
			//_presenterItem.ClickEvent += (s, e) => OnShowProperties();
			_presenterItem.Title = GetTooltip();
		}

		private void OnPropertyChanged()
		{
			if (_presenterItem != null)
			{
				_presenterItem.Title = GetTooltip();
				_presenterItem.InvalidatePainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}
		private string GetTooltip()
		{
			if (_device == null)
				return null;
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(_device.PresentationAddressAndDriver);
			stringBuilder.Append(" - ");
			stringBuilder.AppendLine(_device.Driver.ShortName);
			stringBuilder.AppendLine(_device.DeviceState.StateClass.ToDescription());

			return stringBuilder.ToString().TrimEnd();
		}

		protected override Brush GetBrush()
		{
			return _brush;
		}
		public override void Transform()
		{
			base.Transform();
		}

		private ContextMenu CreateContextMenu()
		{
			return _contextMenu;
		}
	}
}