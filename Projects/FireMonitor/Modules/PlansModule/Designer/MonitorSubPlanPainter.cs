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
using XFiresecAPI;

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
					color = GetStateColor(planViewModel.StateClass);
			}
			return PainterCache.GetTransparentBrush(color);
		}

		public Color GetStateColor(XStateClass stateClass)
		{
			switch (stateClass)
			{
				case XStateClass.Attention:
					return Colors.Yellow;
				case XStateClass.Fire1:
				case XStateClass.Fire2:
					return Colors.Red;
				case XStateClass.Failure:
					return Colors.Pink;
				case XStateClass.Service:
				case XStateClass.Ignore:
					return Colors.Yellow;
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.ConnectionLost:
				case XStateClass.TechnologicalRegime:
					return Colors.Gray;
				case XStateClass.Info:
					return Colors.LightBlue;
				case XStateClass.Norm:
				default:
					return Colors.Transparent;
			}
		}
	}
}