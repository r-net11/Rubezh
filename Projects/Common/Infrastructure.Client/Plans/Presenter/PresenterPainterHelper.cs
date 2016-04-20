using System;
using System.Windows.Controls;
using System.Windows.Input;
using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Presenter;
using Microsoft.Practices.Prism.Events;

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
			{
				Item.StateClass.StateChanged += OnPropertyChanged;
			}
			
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
			ServiceFactoryBase.Events.GetEvent<TShowEvent>().Publish(Item.UID);
		}
		private bool CanShowInTree()
		{
			return Item != null;
		}

		private void OnShowProperties()
		{
			var viewModel = _painter.CreatePropertiesViewModel();
			if (viewModel != null)
				DialogService.ShowWindow(viewModel);
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

		public MenuItem CreateShowInTreeItem()
		{
			return UIHelper.BuildMenuItem(
				"Показать в дереве",
				"pack://application:,,,/Controls;component/Images/BTree.png",
				_painter.ShowInTreeCommand
			);
		}
		public MenuItem CreateShowPropertiesItem()
		{
			return UIHelper.BuildMenuItem(
				"Свойства",
				"pack://application:,,,/Controls;component/Images/BSettings.png",
				_painter.ShowPropertiesCommand
			);
		}
	}
}