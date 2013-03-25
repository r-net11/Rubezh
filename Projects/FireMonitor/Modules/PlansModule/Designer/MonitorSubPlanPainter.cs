using System;
using System.Linq;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using PlansModule.ViewModels;
using System.Windows.Input;

namespace PlansModule.Designer
{
	class MonitorSubPlanPainter : SubPlanPainter
	{
		PresenterItem _presenterItem;
		SubPlanPainter _painter;
		Guid PlanUID;

		public MonitorSubPlanPainter(PresenterItem presenterItem, Guid planUID)
			: base(presenterItem.Element)
		{
			PlanUID = planUID;
			_presenterItem = presenterItem;
			_painter = (SubPlanPainter)presenterItem.Painter;
			_presenterItem.Title = (presenterItem.Element as ElementSubPlan).Caption;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = null;
			_presenterItem.Cursor = Cursors.Hand;
			//_presenterItem.DoubleClickEvent += (s, e) => ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(((ElementSubPlan)_presenterItem.Element).PlanUID);
			_presenterItem.ClickEvent += (s, e) => ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(((ElementSubPlan)_presenterItem.Element).PlanUID);
			ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Subscribe(OnPlanStateChanged);
		}

		private void OnPlanStateChanged(Guid planUID)
		{
			if (PlanUID == planUID)
			{
				_presenterItem.InvalidatePainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}

		protected override Brush GetBrush()
		{
			Color color = Colors.Transparent;
			if (PlanTreeViewModel.Current != null)
			{
				var planViewModel = PlanTreeViewModel.Current.AllPlans.FirstOrDefault(x => x.Plan.UID == PlanUID);
				if(planViewModel != null)
					color = GetStateColor(planViewModel.StateType);
			}
			return PainterCache.GetTransparentBrush(color);
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
					return Colors.LightBlue;
				case StateType.Norm:
				default:
					return Colors.Transparent;
			}
		}
	}
}