using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Infrastructure.Plans.Presenter
{
	public class PresenterPainterHelper<T, TShowEvent>
		where T : IStateProvider
		where TShowEvent : CompositePresentationEvent<Guid>, new()
	{
		IBasePainter<T, TShowEvent> _painter;
		ContextMenu _contextMenu;
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

		void UpdateTooltip()
		{
			if (Item != null && Tooltip != null)
				Tooltip.OnStateChanged();
		}

		void OnShowInTree()
		{
			ServiceFactoryBase.Events.GetEvent<TShowEvent>().Publish(Item.UID);
		}

		bool CanShowInTree()
		{
			return Item != null;
		}

		void OnShowProperties()
		{
			var viewModel = _painter.CreatePropertiesViewModel();
			if (viewModel != null)
				DialogService.ShowWindow(viewModel);
		}

		void OnPropertyChanged()
		{
			if (PresenterItem != null)
			{
				UpdateTooltip();
				PresenterItem.InvalidatePainter();
				PresenterItem.DesignerCanvas.Refresh();
			}
		}

		ContextMenu ContextMenu
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