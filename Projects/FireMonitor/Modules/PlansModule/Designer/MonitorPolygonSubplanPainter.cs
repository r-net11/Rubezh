using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using RubezhAPI.GK;
using Infrastructure;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using PlansModule.ViewModels;

namespace PlansModule.Designer
{
	class MonitorPolygonSubPlanPainter : PolygonSubPlanPainter
	{
		MonitorSubPlanPresenterItem _presenterItem;
		SubPlanPainter _painter;
		Guid PlanUID;

		public MonitorPolygonSubPlanPainter(MonitorSubPlanPresenterItem presenterItem, Guid planUID)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			PlanUID = planUID;
			_presenterItem = presenterItem;
			_painter = (SubPlanPainter)presenterItem.Painter;
			_presenterItem.Title = (presenterItem.Element as IElementSubPlan).Caption;
			_presenterItem.PlanViewModel = PlanTreeViewModel.Current.AllPlans.FirstOrDefault(x => x.Plan.UID == PlanUID);
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = null;
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(((IElementSubPlan)_presenterItem.Element).PlanUID);
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
				if (_presenterItem.PlanViewModel != null)
					color = GetStateColor(_presenterItem.PlanViewModel.NamedStateClass.StateClass);
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
				case XStateClass.Test:
					return Colors.LightBlue;
				case XStateClass.Norm:
				default:
					return Colors.Transparent;
			}
		}
	}

	public class MonitorPolygonSubPlanPresenterItem : PresenterItem
	{
		public MonitorPolygonSubPlanPresenterItem(ElementBase element)
			: base(element)
		{
			;
		}

		public PlanViewModel PlanViewModel;

		protected override object GetToolTip()
		{
			return new MonitorSubPlanToolTipViewModel(PlanViewModel);
		}
	}

}