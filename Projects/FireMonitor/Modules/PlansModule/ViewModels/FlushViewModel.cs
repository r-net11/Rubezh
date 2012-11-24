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
		public PresenterItem PresenterItem { get; private set; }
		public double CenterX { get; private set; }
		public double CenterY { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public double Left { get; private set; }
		public double Top { get; private set; }

		public void SetPresenterItem(PresenterItem presenterItem)
		{
			PresenterItem = presenterItem;
			var position = presenterItem.Element.Position;
			CenterX = position.X;
			CenterY = position.Y;
			var rect = presenterItem.Element.GetRectangle();
			Width = rect.Width;
			Height = rect.Height;
			Left = rect.Left;
			Top = rect.Top;
			OnPropertyChanged(() => PresenterItem);
			OnPropertyChanged(() => CenterX);
			OnPropertyChanged(() => CenterY);
			OnPropertyChanged(() => Width);
			OnPropertyChanged(() => Height);
			OnPropertyChanged(() => Left);
			OnPropertyChanged(() => Top);
		}
	}
}
