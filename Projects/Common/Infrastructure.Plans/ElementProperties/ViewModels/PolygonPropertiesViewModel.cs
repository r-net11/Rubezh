using Controls.Converters;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;

namespace Infrastructure.Plans.ElementProperties.ViewModels
{
	public class PolygonPropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		ElementPolygon _elementPolygon;
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public PolygonPropertiesViewModel(ElementPolygon elementPolygon)
		{
			Title = "Свойства фигуры: Полигон";
			_elementPolygon = elementPolygon;
			var position = elementPolygon.GetPosition();
			Left = (int)(position.X * _sensivityFactor);
			Top = (int)(position.Y * _sensivityFactor);
			ImagePropertiesViewModel = new ImagePropertiesViewModel(_elementPolygon);
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
			ElementBase.Copy(this._elementPolygon, this);
			StrokeThickness = _elementPolygon.BorderThickness;
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
			ElementBase.Copy(this, this._elementPolygon);
			var colorConverter = new ColorToSystemColorConverter();
			_elementPolygon.SetPosition(new System.Windows.Point((double)Left / _sensivityFactor, (double)Top / _sensivityFactor));
			_elementPolygon.BorderColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BorderColor, this.BorderColor.GetType(), null, null);
			_elementPolygon.BackgroundColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BackgroundColor, this.BackgroundColor.GetType(), null, null);
			_elementPolygon.BorderThickness = StrokeThickness;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}