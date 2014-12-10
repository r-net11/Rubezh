using System.Windows.Controls;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using SKDModule.ViewModels;
using FiresecAPI.GK;

namespace SKDModule.Plans.Designer
{
	class DoorPainter : BasePointPainter<SKDDoor, ShowSKDDoorEvent>
	{
		private DoorViewModel _doorViewModel;

		public DoorPainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_doorViewModel = new ViewModels.DoorViewModel(Item);
		}

		protected override SKDDoor CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementDoor;
			return element == null ? null : PlanPresenter.Cache.Get<SKDDoor>(element.DoorUID);
		}
		protected override StateTooltipViewModel<SKDDoor> CreateToolTip()
		{
			return new SKDDoorTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);

			var contextMenu = new ContextMenu();
			contextMenu.Items.Add(UIHelper.BuildMenuItem("Открыть", "pack://application:,,,/Controls;component/Images/BTurnOn.png", _doorViewModel.OpenCommand));
			contextMenu.Items.Add(UIHelper.BuildMenuItem("Закрыть", "pack://application:,,,/Controls;component/Images/BTurnOff.png", _doorViewModel.CloseCommand));
			contextMenu.Items.Add(UIHelper.BuildMenuItem("Установить режим ОТКРЫТО", "pack://application:,,,/Controls;component/Images/BTurnOn.png", _doorViewModel.OpenForeverCommand));
			contextMenu.Items.Add(UIHelper.BuildMenuItem("Установить режим ЗАКРЫТО", "pack://application:,,,/Controls;component/Images/BTurnOff.png", _doorViewModel.CloseForeverCommand));
			contextMenu.Items.Add(Helper.CreateShowInTreeItem());
			contextMenu.Items.Add(UIHelper.BuildMenuItem("Показать связанные события", "pack://application:,,,/Controls;component/Images/BJournal.png", ShowJournalCommand));
			contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new DoorDetailsViewModel(Item);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				SKDDoor = Item
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		protected override Brush GetBrush()
		{
			var background = PainterCache.GetBrush(GetStateColor());
			return PictureCacheSource.DoorPicture.GetBrush(background);
		}

		Color GetStateColor()
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