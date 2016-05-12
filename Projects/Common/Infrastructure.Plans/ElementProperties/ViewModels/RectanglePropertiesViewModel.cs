using Controls.Converters;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Events;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;
using System.Windows.Media;

namespace Infrastructure.Plans.ElementProperties.ViewModels
{
	public class RectanglePropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }
		protected ElementRectangle ElementRectangle { get; private set; }

		public RectanglePropertiesViewModel(ElementRectangle element)
		{
			Title = "Свойства фигуры: Прямоугольник";
			ElementRectangle = element;
			Left = (int)(ElementRectangle.Left * _sensivityFactor);
			Top = (int)(ElementRectangle.Top * _sensivityFactor);
			ImagePropertiesViewModel = new ImagePropertiesViewModel(ElementRectangle);
			BindStrokeThicknessCommand = new RelayCommand(OnBindStrokeThickness);
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
		protected virtual void CopyProperties()
		{
			ElementBase.Copy(this.ElementRectangle, this);
			StrokeThickness = ElementRectangle.BorderThickness;

			PlanElementBindingItems = new List<PlanElementBindingItem>();
			if (ElementRectangle.PlanElementBindingItems != null && ElementRectangle.PlanElementBindingItems.Count > 0)
				foreach (var planElementBindingItem in ElementRectangle.PlanElementBindingItems)
				{
					var planElementBindingItemClone = new PlanElementBindingItem()
					{
						PropertyName = planElementBindingItem.PropertyName,
						GlobalVariableUID = planElementBindingItem.GlobalVariableUID
					};
					PlanElementBindingItems.Add(planElementBindingItemClone);
				}
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
				OnPropertyChanged(() => BorderColor);
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

		public List<PlanElementBindingItem> PlanElementBindingItems { get; set; }

		public RelayCommand BindStrokeThicknessCommand { get; private set; }
		void OnBindStrokeThickness()
		{
			ServiceFactoryBase.Events.GetEvent<EditPlanElementBindingEvent>().Publish(EditPlanElementBindingEventArgs.Create(PlanElementBindingItems, () => StrokeThickness));
		}

		protected override bool Save()
		{
			ElementRectangle.Left = (double)Left / _sensivityFactor;
			ElementRectangle.Top = (double)Top / _sensivityFactor;
			ElementBase.Copy(this, this.ElementRectangle);

			var colorConverter = new ColorToSystemColorConverter();
			ElementRectangle.BackgroundColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BackgroundColor, this.BackgroundColor.GetType(), null, null);
			ElementRectangle.BorderColor = (RubezhAPI.Color)colorConverter.ConvertBack(this.BorderColor, this.BorderColor.GetType(), null, null);
			ElementRectangle.BorderThickness = StrokeThickness;
			ImagePropertiesViewModel.Save();
			ElementRectangle.PlanElementBindingItems = PlanElementBindingItems;
			return base.Save();
		}
	}
}