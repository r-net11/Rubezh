using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.Events;
using Infrustructure.Plans.Elements;
using RubezhAPI.Models;
using System.Collections.Generic;
using System.Windows.Media;

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
			BindStrokeThicknessCommand = new RelayCommand(OnBindStrokeThickness);
			CopyProperties();
		}

		protected virtual void CopyProperties()
		{
			ElementBase.Copy(this.ElementRectangle, this);
			StrokeThickness = ElementRectangle.BorderThickness;

			PlanElementBindingItems = new List<PlanElementBindingItem>();
			if (ElementRectangle.PlanElementBindingItems != null && ElementRectangle.PlanElementBindingItems.Count > 0)
				foreach(var planElementBindingItem in ElementRectangle.PlanElementBindingItems)
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
			ElementBase.Copy(this, this.ElementRectangle);
			ElementRectangle.BorderThickness = StrokeThickness;
			ImagePropertiesViewModel.Save();
			ElementRectangle.PlanElementBindingItems = PlanElementBindingItems;
			return base.Save();
		}
	}
}