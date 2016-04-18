using System.Windows.Controls;
using Common;
using RubezhAPI.GK;
using GKModule.ViewModels;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;

namespace GKModule.Plans.Designer
{
	class GKSKDZonePainter : BaseZonePainter<GKSKDZone, ShowGKSKDZoneEvent>
	{
		private SKDZoneViewModel _skdZoneViewModel;

		public GKSKDZonePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_skdZoneViewModel = new SKDZoneViewModel(Item);
		}

		protected override GKSKDZone CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as IElementZone;
			return element == null ? null : PlanPresenter.Cache.Get<GKSKDZone>(element.ZoneUID);
		}
		protected override StateTooltipViewModel<GKSKDZone> CreateToolTip()
		{
			return new SKDZoneTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			if (Item != null)
			{
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Открыть",
					"pack://application:,,,/Controls;component/Images/BTurnOff.png",
					_skdZoneViewModel.OpenCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Закрыть",
					"pack://application:,,,/Controls;component/Images/BTurnOff.png",
					_skdZoneViewModel.CloseCommand));
				contextMenu.Items.Add(Helper.CreateShowInTreeItem());
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать связанные события",
					"pack://application:,,,/Controls;component/Images/BJournal.png",
					_skdZoneViewModel.ShowJournalCommand
				));
				contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			}
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new SKDZoneDetailsViewModel(Item);
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

				case XStateClass.Off:
					return Colors.Green;
				case XStateClass.TurningOff:
					return Colors.LightGreen;
				case XStateClass.Norm:
				case XStateClass.On:
					return Colors.Blue;

				case XStateClass.AutoOff:
					return Colors.Gray;
				case XStateClass.Ignore:
					return Colors.Yellow;
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