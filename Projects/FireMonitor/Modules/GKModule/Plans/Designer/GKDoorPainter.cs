﻿using System.Windows.Controls;
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
using FiresecAPI.GK;
using GKModule.ViewModels;

namespace GKModule.Plans.Designer
{
	class GKDoorPainter : BasePointPainter<GKDoor, ShowGKDoorEvent>
	{
		private DoorViewModel _doorViewModel;

		public GKDoorPainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_doorViewModel = new ViewModels.DoorViewModel(Item);
		}

		protected override GKDoor CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementGKDoor;
			return element == null ? null : PlanPresenter.Cache.Get<GKDoor>(element.DoorUID);
		}
		protected override StateTooltipViewModel<GKDoor> CreateToolTip()
		{
			return new GKDoorTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);

			var contextMenu = new ContextMenu();
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
			var showArchiveEventArgs = new ShowArchiveEventArgs()
			{
				GKDoor = Item
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}

		protected override Brush GetBrush()
		{
			var background = PainterCache.GetBrush(GetStateColor());
			return PictureCacheSource.DoorPicture.GetBrush(background);
		}

		protected Color GetStateColor()
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