using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrastructure.Designer.ElementProperties.ViewModels
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
			ElementBase.Copy(this._elementPolyline, this);
			StrokeThickness = _elementPolyline.BorderThickness;
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
			ElementBase.Copy(this, this._elementPolyline);
			_elementPolyline.BorderThickness = StrokeThickness;
			return base.Save();
		}
	}
}