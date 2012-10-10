using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class PolylinePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementPolyline _elementPolyline;

		public PolylinePropertiesViewModel(ElementPolyline elementPolyline)
		{
			Title = "Свойства фигуры: Линия";
			_elementPolyline = elementPolyline;
			CopyProperties();
		}

		void CopyProperties()
		{
			BorderColor = _elementPolyline.BorderColor;
			StrokeThickness = _elementPolyline.BorderThickness;
		}

		Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged("BorderColor");
			}
		}

		double _strokeThickness;
		public double StrokeThickness
		{
			get { return _strokeThickness; }
			set
			{
				_strokeThickness = value;
				OnPropertyChanged("StrokeThickness");
			}
		}

		protected override bool Save()
		{
			_elementPolyline.BorderColor = BorderColor;
			_elementPolyline.BorderThickness = StrokeThickness;
			return base.Save();
		}
	}
}
