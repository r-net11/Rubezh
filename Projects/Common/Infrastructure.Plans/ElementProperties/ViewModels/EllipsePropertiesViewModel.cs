using Controls.Converters;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrastructure.Plans.ElementProperties.ViewModels
{
	public class EllipsePropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		ElementEllipse _elementEllipse;
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public EllipsePropertiesViewModel(ElementEllipse element)
		{
			Title = "Свойства фигуры: Эллипс";
			_elementEllipse = element;
			Left = (int)(_elementEllipse.Left * _sensivityFactor);
			Top = (int)(_elementEllipse.Top * _sensivityFactor);
			ImagePropertiesViewModel = new ImagePropertiesViewModel(_elementEllipse);
			CopyProperties();
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}

		void CopyProperties()
		{
			ElementBase.Copy(this._elementEllipse, this);
			StrokeThickness = _elementEllipse.BorderThickness;
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
				OnPropertyChanged(() => BorderColor);
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
				OnPropertyChanged(() => StrokeThickness);
			}
		}

		bool _showTooltip;
		public bool ShowTooltip
		{
			get { return this._showTooltip; }
			set
			{
				this._showTooltip = value;
				OnPropertyChanged(() => this.ShowTooltip);
			}
		}

		protected override bool Save()
		{
			_elementEllipse.Left = (double)Left / _sensivityFactor;
			_elementEllipse.Top = (double)Top / _sensivityFactor;
			ElementBase.Copy(this, this._elementEllipse);
			var colorConverter = new ColorToSystemColorConverter();
			_elementEllipse.BorderColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BorderColor, this.BorderColor.GetType(), null, null);
			_elementEllipse.BackgroundColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BackgroundColor, this.BackgroundColor.GetType(), null, null);
			_elementEllipse.BorderThickness = StrokeThickness;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}