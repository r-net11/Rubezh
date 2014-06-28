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
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using System;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.Plans.Designer
{
	class XZonePainter : BaseZonePainter<XZone, ShowXZoneEvent>
	{
		private ZoneViewModel _zoneViewModel;

		public XZonePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_zoneViewModel = new ViewModels.ZoneViewModel(Item);
		}

		protected override XZone CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as IElementZone;
			return element == null ? null : PlanPresenter.Cache.Get<XZone>(element.ZoneUID);
		}
		protected override StateTooltipViewModel<XZone> CreateToolTip()
		{
			return new ZoneTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			if (Item != null)
			{
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать в дереве",
					"pack://application:,,,/Controls;component/Images/BTree.png",
					ShowInTreeCommand
				));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Отключить все устройства",
					"pack://application:,,,/Controls;component/Images/BTurnOff.png",
					_zoneViewModel.SetIgnoreAllCommand
				));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Снять отключения всех устройств",
					"pack://application:,,,/Controls;component/Images/BResetIgnore.png",
					_zoneViewModel.ResetIgnoreAllCommand
				));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать связанные события",
					"pack://application:,,,/Controls;component/Images/BJournal.png",
					_zoneViewModel.ShowJournalCommand
				));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Свойства",
					"pack://application:,,,/Controls;component/Images/BSettings.png",
					_zoneViewModel.ShowPropertiesCommand
				));
			}
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new ZoneDetailsViewModel(Item);
		}
		protected override Guid ItemUID
		{
			get { return Item.BaseUID; }
		}
	}
}