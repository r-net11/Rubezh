using System.Windows.Controls;
using FiresecAPI.SKD;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;
using SKDModule.Events;
using SKDModule.ViewModels;
using Infrastructure.Events;
using System.Windows.Media;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure;

namespace SKDModule.Plans.Designer
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
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Открыть", "pack://application:,,,/Controls;component/Images/BTurnOn.png", _zoneViewModel.OpenCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Закрыть", "pack://application:,,,/Controls;component/Images/BTurnOff.png", _zoneViewModel.CloseCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Установить режим ОТКРЫТО", "pack://application:,,,/Controls;component/Images/BTurnOn.png", _zoneViewModel.OpenForeverCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem("Установить режим ЗАКРЫТО", "pack://application:,,,/Controls;component/Images/BTurnOff.png", _zoneViewModel.CloseForeverCommand));
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
				Zone = Item
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