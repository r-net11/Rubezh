using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class PolylinePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementPolyline _elementPolyline;

		public PolylinePropertiesViewModel(ElementPolyline elementPolyline)
		{
            Title = Resources.Language.ElementProperties.ViewModels.PolylinePropertiesViewModel.Title;
			_elementPolyline = elementPolyline;
			CopyProperties();
		}

		void CopyProperties()
		{
			BorderColor = _elementPolyline.BorderColor;
			StrokeThickness = _elementPolyline.BorderThickness;
			PresentationName = _elementPolyline.PresentationName;
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

		string _presentationName;
		public string PresentationName
		{
			get { return _presentationName; }
			set
			{
				_presentationName = value;
				OnPropertyChanged(() => PresentationName);
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
			_elementPolyline.PresentationName = PresentationName;
			return base.Save();
		}
	}
}
