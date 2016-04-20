using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

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
			ElementBase.Copy(this, this._elementEllipse);
			_elementEllipse.BorderThickness = StrokeThickness;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}