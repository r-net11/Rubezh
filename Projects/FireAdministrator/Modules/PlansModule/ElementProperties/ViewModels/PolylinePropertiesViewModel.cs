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
			BackgroundColor = _elementPolyline.BackgroundColor;
			StrokeThickness = _elementPolyline.BorderThickness;
		}

		Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged("BackgroundColor");
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
			_elementPolyline.BackgroundColor = BackgroundColor;
			_elementPolyline.BorderThickness = StrokeThickness;
			return base.Save();
		}
	}
}
