using Controls.Converters;
using Controls.Extentions;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrastructure.Plans.ElementProperties.ViewModels
{
	public class EllipsePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementEllipse _elementEllipse;
		public PositionSettingsViewModel PositionSettingsViewModel { get; private set; }
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public EllipsePropertiesViewModel(ElementEllipse element, CommonDesignerCanvas designerCanvas)
		{
			Title = "Свойства фигуры: Эллипс";
			_elementEllipse = element;
			PositionSettingsViewModel = new PositionSettingsViewModel(element as ElementBase, designerCanvas);
			ImagePropertiesViewModel = new ImagePropertiesViewModel(_elementEllipse);
			CopyProperties();
		}

		void CopyProperties()
		{
			ElementBase.Copy(this._elementEllipse, this);
			StrokeThickness = _elementEllipse.BorderThickness;
			BackgroundColor = _elementEllipse.BackgroundColor.ToWindowsColor();
			BorderColor = _elementEllipse.BorderColor.ToWindowsColor();
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
			PositionSettingsViewModel.SavePosition();
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