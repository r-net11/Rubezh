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
	class MonitorSubPlanPainter : SubPlanPainter
	{
		private PresenterItem _presenterItem;
		private PlanViewModel _planViewModel;
		private SubPlanPainter _painter;

		public MonitorSubPlanPainter(PresenterItem presenterItem, PlanViewModel planViewModel)
			: base(presenterItem.Element)
		{
			_planViewModel = planViewModel;
			_presenterItem = presenterItem;
			_painter = (SubPlanPainter)presenterItem.Painter;
			_presenterItem.Title = (presenterItem.Element as ElementSubPlan).Caption;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = null;
			_presenterItem.DoubleClickEvent += (s, e) => ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(((ElementSubPlan)_presenterItem.Element).PlanUID);
			ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Subscribe(OnPlanStateChanged);
		}

		private void OnPlanStateChanged(Guid planUID)
		{
			if (_planViewModel != null && _planViewModel.Plan.UID == planUID)
			{
				_presenterItem.RefreshPainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}

		protected override Brush GetBrush()
		{
			var color = GetStateColor(_planViewModel.StateType);
			return PainterCache.GetTransparentBrush(color, Element.BackgroundPixels);
		}

		public Color GetStateColor(StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Fire:
					return Colors.Red;
				case StateType.Attention:
					return Colors.Yellow;
				case StateType.Failure:
					return Colors.Pink;
				case StateType.Service:
				case StateType.Off:
					return Colors.Yellow;
				case StateType.Unknown:
					return Colors.Gray;
				case StateType.Info:
				case StateType.Norm:
				default:
					return Colors.Transparent;
			}
		}
	}
}
