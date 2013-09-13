using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using System.Windows.Controls;
using Controls.Converters;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using FiresecAPI.Models;
using System.Windows.Input;
using System.Diagnostics;

namespace GKModule.Plans.Designer
{
	class XZonePainter : PolygonZonePainter, IPainter
	{
		PresenterItem _presenterItem;
		XZone Zone;
		ContextMenu _contextMenu;

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
			_presenterItem.Title = GetZoneTooltip();
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
		}

		void OnPropertyChanged()
		{
			_presenterItem.Title = GetZoneTooltip();
			_presenterItem.InvalidatePainter();
			_presenterItem.DesignerCanvas.Refresh();
		}
		string GetZoneTooltip()
		{
			if (Zone == null)
				return null;
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Zone.PresentationName);
			stringBuilder.AppendLine("Состояние: " + Zone.ZoneState.StateClass.ToDescription());
			return stringBuilder.ToString().TrimEnd();
		}

		#region IPainter Members

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
				case XStateClass.ConnectionLost:
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
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(Zone.UID);
		}
		bool CanShowInTree()
		{
			return Zone != null;
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Zone = Zone
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(Zone);
			DialogService.ShowWindow(zoneDetailsViewModel);
		}

		ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в списке",
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать связанные события",
					Command = ShowJournalCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Свойства",
					Command = ShowPropertiesCommand
				});
			}
			return _contextMenu;
		}
	}
}