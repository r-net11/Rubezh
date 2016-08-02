using System.Windows.Media;
using StrazhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class EllipsePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementEllipse _elementEllipse;
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public EllipsePropertiesViewModel(ElementEllipse elementEllipse)
		{
			Title = "Свойства фигуры: Эллипс";
			_elementEllipse = elementEllipse;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(_elementEllipse);
			CopyProperties();
		}

		void CopyProperties()
		{
			BackgroundColor = _elementEllipse.BackgroundColor.ToWindowsColor();
			BorderColor = _elementEllipse.BorderColor.ToWindowsColor();
			StrokeThickness = _elementEllipse.BorderThickness;
			PresentationName = _elementEllipse.PresentationName;
		}

		Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
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
			_elementEllipse.BackgroundColor = BackgroundColor.ToStruzhColor();
			_elementEllipse.BorderColor = BorderColor.ToStruzhColor();
			_elementEllipse.BorderThickness = StrokeThickness;
			_elementEllipse.PresentationName = PresentationName;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}
