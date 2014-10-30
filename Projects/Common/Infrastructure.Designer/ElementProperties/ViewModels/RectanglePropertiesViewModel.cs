using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class RectanglePropertiesViewModel : SaveCancelDialogViewModel
	{
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }
		protected ElementRectangle ElementRectangle { get; private set; }

		public RectanglePropertiesViewModel(ElementRectangle elementRectangle)
		{
			Title = "Свойства фигуры: Прямоугольник";
			ElementRectangle = elementRectangle;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(ElementRectangle);
			CopyProperties();
		}

		protected virtual void CopyProperties()
		{
			BackgroundColor = ElementRectangle.BackgroundColor;
			BorderColor = ElementRectangle.BorderColor;
			StrokeThickness = ElementRectangle.BorderThickness;
			PresentationName = ElementRectangle.PresentationName;
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
			ElementRectangle.BackgroundColor = BackgroundColor;
			ElementRectangle.BorderColor = BorderColor;
			ElementRectangle.BorderThickness = StrokeThickness;
			ElementRectangle.PresentationName = PresentationName;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}