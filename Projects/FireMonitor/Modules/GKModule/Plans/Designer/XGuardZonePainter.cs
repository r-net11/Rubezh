using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;

namespace GKModule.Plans.Designer
{
	class XGuardZonePainter : PolygonZonePainter, IPainter
	{
		private PresenterItem _presenterItem;
		private XGuardZone GuardZone;
		private GuardZoneViewModel GuardZoneViewModel;
		private ContextMenu _contextMenu;
		private GuardZoneTooltipViewModel _tooltip;

		public XGuardZonePainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			_contextMenu = null;
			_presenterItem = presenterItem;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			GuardZone = Helper.GetXGuardZone((IElementZone)_presenterItem.Element);
			if (GuardZone != null)
			{
				GuardZoneViewModel = new ViewModels.GuardZoneViewModel(GuardZone);
				GuardZone.State.StateChanged += OnPropertyChanged;
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
			if (GuardZone != null)
			{
				if (_tooltip == null)
				{
					_tooltip = new GuardZoneTooltipViewModel(GuardZone);
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
			switch (GuardZone.State.StateClass)
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
					return Colors.Brown;

				default:
					return Colors.White;
			}
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		private void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXGuardZoneEvent>().Publish(GuardZone.BaseUID);
		}
		private bool CanShowInTree()
		{
			return GuardZone != null;
		}

		void ShowProperties()
		{
			DialogService.ShowWindow(new GuardZoneDetailsViewModel(GuardZone));
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				if (GuardZone != null)
				{
					ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);

					_contextMenu = new ContextMenu();
					_contextMenu.Items.Add(Helper.BuildMenuItem(
						"Показать в дереве",
						"pack://application:,,,/Controls;component/Images/BTree.png",
						ShowInTreeCommand
					));

					_contextMenu.Items.Add(Helper.BuildMenuItem(
						"Показать связанные события",
						"pack://application:,,,/Controls;component/Images/BJournal.png",
						GuardZoneViewModel.ShowJournalCommand
					));
					_contextMenu.Items.Add(Helper.BuildMenuItem(
						"Свойства",
						"pack://application:,,,/Controls;component/Images/BSettings.png",
						GuardZoneViewModel.ShowPropertiesCommand
					));
				}
			}
			return _contextMenu;
		}
	}
}