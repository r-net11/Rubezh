using Infrastructure;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Painters;
using Infrastructure.Plans.Presenter;
using PlansModule.ViewModels;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Color = RubezhAPI.Color;
using Colors = RubezhAPI.Colors;

namespace PlansModule.Designer
{
	class MonitorSubPlanPainter : SubPlanPainter
	{
		MonitorSubPlanPresenterItem _presenterItem;
		SubPlanPainter _painter;
		Guid PlanUID;

		public MonitorSubPlanPainter(MonitorSubPlanPresenterItem presenterItem, Guid planUID)
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
			//_presenterItem.DoubleClickEvent += (s, e) => ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(((ElementSubPlan)_presenterItem.Element).PlanUID);
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

	public class MonitorSubPlanPresenterItem : PresenterItem
	{
		public MonitorSubPlanPresenterItem(ElementBase element)
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