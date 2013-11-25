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

namespace GKModule.Plans.Designer
{
	class XZonePainter : PolygonZonePainter, IPainter
	{
		private PresenterItem _presenterItem;
		private XZone Zone;
		private ContextMenu _contextMenu;
		private ImageTextStateTooltipViewModel _tooltip;

		public XZonePainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			Zone = Helper.GetXZone((IElementZone)_presenterItem.Element);
			if (Zone != null)
				Zone.ZoneState.StateChanged += OnPropertyChanged;
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
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
			if (Zone == null)
				return;

			if (_tooltip == null)
			{
				_tooltip = new ImageTextStateTooltipViewModel();
				_tooltip.TitleViewModel.Title = Zone.PresentationName.TrimEnd();
				_tooltip.TitleViewModel.ImageSource = @"/Controls;component/Images/zone.png";
			}
			_tooltip.StateViewModel.Title = Zone.ZoneState.StateClass.ToDescription();
			_tooltip.StateViewModel.ImageSource = Zone.ZoneState.StateClass.ToIconSource();
			_tooltip.Update();
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
			switch (Zone.ZoneState.StateClass)
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

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Zone = Zone
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(Zone);
			DialogService.ShowWindow(zoneDetailsViewModel);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Показать в дереве", 
					"pack://application:,,,/Controls;component/Images/BTree.png", 
					ShowInTreeCommand
				));
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Показать связанные события", 
					"pack://application:,,,/Controls;component/Images/BJournal.png", 
					ShowJournalCommand
				));
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Свойства", 
					"pack://application:,,,/Controls;component/Images/BSettings.png", 
					ShowPropertiesCommand
				));
				
			}
			return _contextMenu;
		}
	}
}