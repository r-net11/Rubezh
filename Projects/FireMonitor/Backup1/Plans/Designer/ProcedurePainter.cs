using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using FiresecAPI.Automation;
using System.Windows.Input;
using AutomationModule.ViewModels;

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
			PresenterItem.ClickEvent += (s, e) => ProcedureArgumentsViewModel.Run(Item);
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