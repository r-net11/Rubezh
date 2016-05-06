using System.Windows.Controls;
using System.Windows.Media;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;
using StrazhModule.ViewModels;

namespace StrazhModule.Plans.Designer
{
	class SKDZonePainter : BaseZonePainter<SKDZone, ShowSKDZoneEvent>
	{
		private ZoneViewModel _zoneViewModel;

		public SKDZonePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_zoneViewModel = new ViewModels.ZoneViewModel(Item);
		}

		protected override SKDZone CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as IElementZone;
			return element == null ? null : PlanPresenter.Cache.Get<SKDZone>(element.ZoneUID);
		}
		protected override StateTooltipViewModel<SKDZone> CreateToolTip()
		{
			return new SKDZoneTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);

			var contextMenu = new ContextMenu();
			if (Item != null)
			{
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Сброс состояния \"Взлом\"", null, _zoneViewModel.ClearPromptWarningCommand));
				contextMenu.Items.Add(new Separator());
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Режим Открыто", null, _zoneViewModel.ZoneAccessStateOpenAlwaysCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Режим Норма", null, _zoneViewModel.ZoneAccessStateNormalCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Режим Закрыто", null, _zoneViewModel.ZoneAccessStateCloseAlwaysCommand));
				contextMenu.Items.Add(new Separator());
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Открыть", null, _zoneViewModel.OpenCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Закрыть", null, _zoneViewModel.CloseCommand));
				contextMenu.Items.Add(new Separator());
				contextMenu.Items.Add(Helper.CreateShowInTreeItem());
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Показать связанные события", "pack://application:,,,/Controls;component/Images/BJournal.png", ShowJournalCommand));
				contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			}
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new ZoneDetailsViewModel(Item);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				SKDZone = Item
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
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
					return Colors.Blue;
				case XStateClass.TurningOn:
					return Colors.LightBlue;
				case XStateClass.Norm:
				case XStateClass.Off:
					return Colors.Green;

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