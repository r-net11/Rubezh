using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Presenter;
using System.Windows;

namespace PlansModule.ViewModels
{
	public class FlushViewModel : BaseViewModel
	{
		private double _pointZoom;
		public PresenterItem PresenterItem { get; private set; }
		public double CenterX { get; private set; }
		public double CenterY { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public double Left { get; private set; }
		public double Top { get; private set; }
		public double Thickness { get; private set; }

		public void SetPresenterItem(PresenterItem presenterItem)
		{
			PresenterItem = presenterItem;
			SetPresenterItem();
		}
		private void SetPresenterItem()
		{
			var position = PresenterItem.Element.Position;
			CenterX = position.X;
			CenterY = position.Y;
			var rect = PresenterItem.Element.GetRectangle();
			Width = PresenterItem.IsPoint ? _pointZoom : rect.Width;
			Height = PresenterItem.IsPoint ? _pointZoom : rect.Height;
			Left = rect.Left;
			Top = rect.Top;
			if (PresenterItem.IsPoint)
			{
				Left -= _pointZoom / 2;
				Top -= _pointZoom / 2;
			}
			OnPropertyChanged(() => PresenterItem);
			OnPropertyChanged(() => CenterX);
			OnPropertyChanged(() => CenterY);
			OnPropertyChanged(() => Width);
			OnPropertyChanged(() => Height);
			OnPropertyChanged(() => Left);
			OnPropertyChanged(() => Top);
		}
		public void UpdateDeviceZoom(double zoom, double pointZoom)
		{
			_pointZoom = pointZoom;
			Thickness = 5 / zoom;
			OnPropertyChanged(() => Thickness);
			if (PresenterItem != null)
				SetPresenterItem();
		}
		//public void ShowTitle()
		//{
		//    if (!string.IsNullOrEmpty(Title))
		//    {
		//        var toolTip = new ToolTip()
		//        {
		//            Content = Title,
		//            PlacementTarget = this,
		//            Placement = PlacementMode.Right,
		//            IsOpen = true,
		//        };
		//    }
		//}
	}
}
