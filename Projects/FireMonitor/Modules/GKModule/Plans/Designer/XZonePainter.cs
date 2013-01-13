using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Painters;
using System.Windows;
using Infrustructure.Plans.Elements;
using FiresecClient;
using FiresecAPI.Models;
using DeviceControls;
using FiresecAPI;
using Infrustructure.Plans.Presenter;
using System.Windows.Shapes;
using System.Windows.Media;
using XFiresecAPI;
using System.Windows.Controls;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;

namespace GKModule.Plans.Designer
{
	class XZonePainter : IPainter
	{
		private PresenterItem _presenterItem;
		private IPainter _painter;
		private XZone _zone;

		public XZonePainter(PresenterItem presenterItem)
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
			_presenterItem = presenterItem;
			_painter = presenterItem.Painter;
			Bind();
		}

		private void Bind()
		{
			_presenterItem.Border = BorderHelper.CreateBorderPolyline(_presenterItem.Element);
			//_presenterItem.ContextMenu = (ContextMenu)_presenterItem.FindResource("XZoneMenuView");
			//_presenterItem.ContextMenu.DataContext = this;
			_zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == ((IElementZone)_presenterItem.Element).ZoneUID);
			if (_zone != null)
				_zone.ZoneState.StateChanged += OnPropertyChanged;
			_presenterItem.Title = GetZoneTooltip();
		}

		private void OnPropertyChanged()
		{
			_presenterItem.Title = GetZoneTooltip();
			_presenterItem.Redraw();
		}
		private string GetZoneTooltip()
		{
			if (_zone == null)
				return null;
			var sb = new StringBuilder();
			sb.AppendLine(_zone.PresentationName);
			sb.AppendLine("Состояние: " + _zone.ZoneState.GetStateType().ToDescription());
			return sb.ToString().TrimEnd();
		}

		public Brush GetStateBrush()
		{
			return Brushes.Yellow;
		}

		#region IPainter Members

		public bool RedrawOnZoom
		{
			get { return true; }
		}
		public void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
		}
		//public UIElement Draw(ElementBase element)
		//{
		//    if (_zone == null)
		//        return null;
		//    var shape = (Shape)_painter.Draw(element);
		//    shape.Fill = GetStateBrush();
		//    shape.Opacity = 1;
		//    return shape;
		//}

		#endregion

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(_zone.UID);
		}
		bool CanShowInTree()
		{
			return _zone != null;
		}
	}
}
