using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Controls.Converters;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using PlansModule.ViewModels;

namespace PlansModule.Designer
{
	class MonitorSubPlanPainter : IPainter
	{
		private PresenterItem _presenterItem;
		private PlanViewModel _planViewModel;
		private IPainter _painter;
		private StateTypeToColorConverter _converter;

		public MonitorSubPlanPainter(PresenterItem presenterItem, PlanViewModel planViewModel)
		{
			_converter = new StateTypeToColorConverter();
			_planViewModel = planViewModel;
			_presenterItem = presenterItem;
			_painter = presenterItem.Painter;
			_presenterItem.Title = (presenterItem.Element as ElementSubPlan).Caption;
			_presenterItem.Border = BorderHelper.CreateBorderRectangle();
			//_presenterItem.MouseDoubleClick += (s, e) => ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(((ElementSubPlan)_presenterItem.Element).PlanUID);
			ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Subscribe(OnPlanStateChanged);
		}

		private void OnPlanStateChanged(Guid planUID)
		{
			if (_planViewModel != null && _planViewModel.Plan.UID == planUID)
				_presenterItem.Redraw();
		}
		public Brush GetStateBrush()
		{
			StateType stateType = (StateType)_planViewModel.StateType;
			return (Brush)_converter.Convert(stateType, typeof(Brush), null, CultureInfo.CurrentCulture);
		}

		#region IPainter Members

		public bool RedrawOnZoom
		{
			get { return false; }
		}
		public void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
		}
		//public UIElement Draw(ElementBase element)
		//{
		//    var shape = (Shape)_painter.Draw(element);
		//    shape.Fill = GetStateBrush();
		//    shape.Opacity = 0.6;
		//    return shape;
		//}

		#endregion
	}
}
