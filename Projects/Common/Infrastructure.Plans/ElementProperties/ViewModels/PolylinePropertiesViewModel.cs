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
	public class PolylinePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementPolyline _elementPolyline;
		public PositionSettingsViewModel PositionSettingsViewModel { get; private set; }
		public PolylinePropertiesViewModel(ElementPolyline element, CommonDesignerCanvas designerCanvas)
		{
			Title = "Свойства фигуры: Линия";
			_elementPolyline = element;
			PositionSettingsViewModel = new PositionSettingsViewModel(element as ElementBase, designerCanvas);
			CopyProperties();
		}
		void CopyProperties()
		{
			ElementBase.Copy(this._elementPolyline, this);
			StrokeThickness = _elementPolyline.BorderThickness;
			BorderColor = _elementPolyline.BorderColor.ToWindowsColor();
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
			ElementBase.Copy(this, this._elementPolyline);
			var colorConverter = new ColorToSystemColorConverter();
			_elementPolyline.BorderColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BorderColor, this.BorderColor.GetType(), null, null);
			_elementPolyline.BorderThickness = StrokeThickness;
			return base.Save();
		}
	}
}