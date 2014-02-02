using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Controls;
using FiresecAPI;
using FiresecAPI.Models;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client.Plans.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using SKDModule.ViewModels;

namespace SKDModule.Plans.Designer
{
	class SKDZonePainter : PolygonZonePainter, IPainter
	{
		private PresenterItem _presenterItem;
		private SKDZone Zone;
		private ZoneViewModel ZoneViewModel;
		private ContextMenu _contextMenu;
		private ZoneTooltipViewModel _tooltip;

		public SKDZonePainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			Zone = Helper.GetSKDZone((IElementZone)_presenterItem.Element);
			if (Zone != null)
			{
				ZoneViewModel = new ViewModels.ZoneViewModel(Zone);
				Zone.State.StateChanged += OnPropertyChanged;
			}
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => ShowProperties();
			UpdateTooltip();
		}

		private void OnPropertyChanged()
		{
			if (_presenterItem != null)
			{
				UpdateTooltip();
				_presenterItem.InvalidatePainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}
		private void UpdateTooltip()
		{
			if (Zone != null)
			{
				if (_tooltip == null)
				{
					_tooltip = new ZoneTooltipViewModel(Zone);
				}
			}
		}

		#region IPainter Members
		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(GetStateColor());
		}
		#endregion

		public Color GetStateColor()
		{
			switch (Zone.State.StateClass)
			{
				case XStateClass.Unknown:
				case XStateClass.DBMissmatch:
				case XStateClass.TechnologicalRegime:
				case XStateClass.ConnectionLost:
				case XStateClass.HasNoLicense:
					return Colors.DarkGray;

				case XStateClass.Fire1:
				case XStateClass.Fire2:
					return Colors.Red;

				case XStateClass.Attention:
					return Colors.Yellow;

				case XStateClass.Ignore:
					return Colors.Yellow;

				case XStateClass.Norm:
					return Colors.Green;

				default:
					return Colors.White;
			}
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		private void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(Zone.UID);
		}
		private bool CanShowInTree()
		{
			return Zone != null;
		}

		void ShowProperties()
		{
			DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				if (Zone != null)
				{
					ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);

					_contextMenu = new ContextMenu();
					_contextMenu.Items.Add(Helper.BuildMenuItem(
						"Показать в дереве",
						"pack://application:,,,/Controls;component/Images/BTree.png",
						ShowInTreeCommand
					));

					_contextMenu.Items.Add(Helper.BuildMenuItem(
						"Команда",
						"pack://application:,,,/Controls;component/Images/BTurnOff.png",
						ZoneViewModel.ZoneCommand
					));

					_contextMenu.Items.Add(Helper.BuildMenuItem(
						"Показать связанные события",
						"pack://application:,,,/Controls;component/Images/BJournal.png",
						ZoneViewModel.ShowJournalCommand
					));
					_contextMenu.Items.Add(Helper.BuildMenuItem(
						"Свойства",
						"pack://application:,,,/Controls;component/Images/BSettings.png",
						ZoneViewModel.ShowPropertiesCommand
					));
				}
			}
			return _contextMenu;
		}
	}
}