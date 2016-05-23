using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System;
using System.Linq;

namespace Infrastructure.Plans.ViewModels
{
	public class PositionSettingsViewModel : BaseViewModel
	{
		CommonDesignerCanvas DesignerCanvas { get; set; }
		ElementBase ElementBase { get; set; }
		double ElementWidth { get; set; }
		double ElementHeight { get; set; }
		public PositionSettingsViewModel(ElementBase element, CommonDesignerCanvas designerCanvas)
		{
			ElementBase = element;
			DesignerCanvas = designerCanvas;
			var rectangle = ElementBase.GetRectangle();
			Left = Math.Round(rectangle.X, 3).ToString();
			Top = Math.Round(rectangle.Y, 3).ToString();
			ElementWidth = rectangle.Width;
			ElementHeight = rectangle.Height;
		}

		string _left;
		public string Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		string _top;
		public string Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}
		public void SavePosition()
		{
			if (Left.LastOrDefault() == ',')
				Left += "0";
			if (Top.LastOrDefault() == ',')
				Top += "0";
			double left;
			double top;
			if (double.TryParse(Left, out left) && double.TryParse(Top, out top))
			{
				if (left + ElementWidth > DesignerCanvas.ActualWidth)
					left = DesignerCanvas.ActualWidth - ElementWidth;
				if (top + ElementHeight > DesignerCanvas.ActualHeight)
					top = DesignerCanvas.ActualHeight - ElementHeight;
				ElementBase.SetPosition(new System.Windows.Point(left, top));
			}
		}
	}
}