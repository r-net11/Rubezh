using System.Windows.Media;
using StrazhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class PolygonPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementPolygon _elementPolygon;
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public PolygonPropertiesViewModel(ElementPolygon elementPolygon)
		{
			Title = "Свойства фигуры: Полигон";
			_elementPolygon = elementPolygon;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(_elementPolygon);
			CopyProperties();
		}

		void CopyProperties()
		{
			BackgroundColor = _elementPolygon.BackgroundColor.ToWindowsColor();
			BorderColor = _elementPolygon.BorderColor.ToWindowsColor();
			StrokeThickness = _elementPolygon.BorderThickness;
			PresentationName = _elementPolygon.PresentationName;
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
			_elementPolygon.BackgroundColor = BackgroundColor.ToStruzhColor();
			_elementPolygon.BorderColor = BorderColor.ToStruzhColor();
			_elementPolygon.BorderThickness = StrokeThickness;
			_elementPolygon.PresentationName = PresentationName;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}
