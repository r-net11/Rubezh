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

namespace GKModule.Plans.Designer
{
	class XZonePainter : PolygonZonePainter, IPainter
	{
		private PresenterItem _presenterItem;
		private XZone Zone;
		private ContextMenu _contextMenu;

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

		private void OnPropertyChanged()
		{
			_presenterItem.Title = GetZoneTooltip();
			_presenterItem.InvalidatePainter();
			_presenterItem.DesignerCanvas.Refresh();
		}
		private string GetZoneTooltip()
		{
			if (Zone == null)
				return null;
			var sb = new StringBuilder();
			sb.AppendLine(Zone.PresentationName);
			sb.AppendLine("Состояние: " + Zone.ZoneState.GetStateType().ToDescription());
			return sb.ToString().TrimEnd();
		}

		#region IPainter Members

		protected override Brush GetBrush()
		{
			return PainterCache.GetTransparentBrush(GetStateColor());
		}

		#endregion

		public Color GetStateColor()
		{
			var stateType = Zone.ZoneState.GetStateType();
			switch (stateType)
			{
				case StateType.Fire:
					return Colors.Red;

				case StateType.Attention:
					return Colors.Yellow;

				case StateType.Failure:
					return Colors.Pink;

				case StateType.Service:
					return Colors.Yellow;

				case StateType.Off:
					return Colors.Yellow;

				case StateType.Unknown:
					return Colors.Gray;

				case StateType.Info:
					return Colors.LightBlue;

				case StateType.Norm:
					return Colors.LightGreen;

				case StateType.No:
					return Colors.White;

				default:
					return Colors.Black;
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

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(Zone);
			DialogService.ShowWindow(zoneDetailsViewModel);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);
				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в списке",
					Command = ShowInTreeCommand
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