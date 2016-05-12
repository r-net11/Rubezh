using AutomationModule.ViewModels;
using Infrastructure.Plans.Painters;
using Infrastructure.Plans.Presenter;
using RubezhAPI.Automation;
using RubezhAPI.Models;
using System.Windows.Input;

namespace AutomationModule.Plans.Designer
{
	class ProcedurePainter : TextBlockPainter
	{
		private ProcedureTooltipViewModel _tooltip;
		public PresenterItem PresenterItem { get; private set; }
		public Procedure Item { get; private set; }

		public ProcedurePainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			PresenterItem = presenterItem;
			Item = CreateItem(presenterItem);

			PresenterItem.IsPoint = false;
			PresenterItem.ShowBorderOnMouseOver = true;
			PresenterItem.Cursor = Cursors.Hand;
			PresenterItem.ClickEvent += (s, e) =>
				ProcedureArgumentsViewModel.Run(Item);
			_tooltip = new ProcedureTooltipViewModel(Item);
		}

		private Procedure CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementProcedure;
			return element == null ? null : PlanPresenter.Cache.Get<Procedure>(element.ProcedureUID);
		}
		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
	}
}