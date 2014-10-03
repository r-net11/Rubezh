using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI.GK;
using GKModule.ViewModels;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;

namespace GKModule.Plans.Designer
{
	class XGuardZonePainter : BaseZonePainter<GKGuardZone, ShowXGuardZoneEvent>
	{
		private GuardZoneViewModel _guardZoneViewModel;

		public XGuardZonePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_guardZoneViewModel = new ViewModels.GuardZoneViewModel(Item);
		}

		protected override GKGuardZone CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as IElementZone;
			return element == null ? null : PlanPresenter.Cache.Get<GKGuardZone>(element.ZoneUID);
		}
		protected override StateTooltipViewModel<GKGuardZone> CreateToolTip()
		{
			return new GuardZoneTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			if (Item != null)
			{
				contextMenu.Items.Add(Helper.CreateShowInTreeItem());
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать связанные события",
					"pack://application:,,,/Controls;component/Images/BJournal.png",
					_guardZoneViewModel.ShowJournalCommand
				));
				contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			}
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new GuardZoneDetailsViewModel(Item);
		}

		protected override Color GetStateColor()
		{
			if (Item == null)
				return Colors.Transparent;

			switch (Item.State.StateClass)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
					return Colors.DarkGray;
				case XStateClass.On:
					return Colors.Green;
				case XStateClass.TurningOn:
					return Colors.LightGreen;
				case XStateClass.AutoOff:
					return Colors.Gray;
				case XStateClass.Ignore:
					return Colors.Yellow;
				case XStateClass.Norm:
				case XStateClass.Off:
					return Colors.Blue;
				case XStateClass.Fire1:
				case XStateClass.Fire2:
				case XStateClass.Attention:
					return Colors.Red;
				default:
					return Colors.White;
			}
		}
	}
}