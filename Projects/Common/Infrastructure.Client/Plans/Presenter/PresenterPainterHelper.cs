using System;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Presenter;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Windows;

namespace Infrastructure.Client.Plans.Presenter
{
	public class PresenterPainterHelper<T, TShowEvent>
		where T : IStateProvider
		where TShowEvent : CompositePresentationEvent<Guid>, new()
	{
		private IBasePainter<T, TShowEvent> _painter;
		private ContextMenu _contextMenu;
		public StateTooltipViewModel<T> Tooltip { get; private set; }
		public PresenterItem PresenterItem { get; private set; }
		public T Item { get; private set; }

		public PresenterPainterHelper(PresenterItem presenterItem, IBasePainter<T, TShowEvent> painter)
		{
			_painter = painter;
			PresenterItem = presenterItem;
			Item = _painter.CreateItem(presenterItem);
		}

		public void Initialize()
		{
			if (Item != null && Item.StateClass != null)
				Item.StateClass.StateChanged += OnPropertyChanged;
			PresenterItem.IsPoint = _painter.IsPoint;
			PresenterItem.ShowBorderOnMouseOver = true;
			PresenterItem.Cursor = Cursors.Hand;
			PresenterItem.ClickEvent += (s, e) => OnShowProperties();
			PresenterItem.ContextMenuProvider = () => ContextMenu;
			_painter.ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
			_painter.ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			Tooltip = _painter.CreateToolTip();
			UpdateTooltip();
		}

		private void UpdateTooltip()
		{
			if (Item != null && Tooltip != null)
				Tooltip.OnStateChanged();
		}

		private void OnShowInTree()
		{
			ServiceFactoryBase.Events.GetEvent<TShowEvent>().Publish(_painter.ItemUID);
		}
		private bool CanShowInTree()
		{
			return Item != null;
		}

		private void OnShowProperties()
		{
			DialogService.ShowWindow(_painter.CreatePropertiesViewModel());
		}

		private void OnPropertyChanged()
		{
			if (PresenterItem != null)
			{
				UpdateTooltip();
				PresenterItem.InvalidatePainter();
				PresenterItem.DesignerCanvas.Refresh();
			}
		}
		private ContextMenu ContextMenu
		{
			get
			{
				if (_contextMenu == null)
					_contextMenu = _painter.CreateContextMenu();
				return _contextMenu;
			}
		}
	}
}