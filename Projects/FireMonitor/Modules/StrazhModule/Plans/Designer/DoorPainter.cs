using System.Windows.Controls;
using System.Windows.Media;
using DeviceControls;
using Localization.Strazh.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using StrazhModule.ViewModels;
using StrazhAPI.GK;
using Color = StrazhAPI.Color;
using Colors = StrazhAPI.Colors;

namespace StrazhModule.Plans.Designer
{
	class DoorPainter : BasePointPainter<SKDDoor, ShowSKDDoorEvent>
	{
		private DoorViewModel _doorViewModel;

		public DoorPainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_doorViewModel = new DoorViewModel(Item);
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
			contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ResetBreakinState, null, _doorViewModel.ClearPromptWarningCommand));
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ModeOpen, null, _doorViewModel.DoorAccessStateOpenAlwaysCommand));
			contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ModeNorm, null, _doorViewModel.DoorAccessStateNormalCommand));
			contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ModeClose, null, _doorViewModel.DoorAccessStateCloseAlwaysCommand));
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.Open, null, _doorViewModel.OpenCommand));
			contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.Close, null, _doorViewModel.CloseCommand));
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(Helper.CreateShowInTreeItem());
			contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ShowRelatedEvents, "pack://application:,,,/Controls;component/Images/BJournal.png", ShowJournalCommand));
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