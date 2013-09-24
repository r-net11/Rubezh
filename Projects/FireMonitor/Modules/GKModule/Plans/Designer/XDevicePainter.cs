using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Controls;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using Infrastructure.Client.Plans.ViewModels;

namespace GKModule.Plans.Designer
{
	class XDevicePainter : PointPainter
	{
		private PresenterItem _presenterItem;
		private XDevice Device;
		private ContextMenu _contextMenu;
		private ImageTextStateTooltipViewModel _tooltip;

		public XDevicePainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_contextMenu = null;
			var elementXDevice = presenterItem.Element as ElementXDevice;
			if (elementXDevice != null)
			{
				Device = Helper.GetXDevice(elementXDevice);
				if (Device != null && Device.DeviceState != null)
					Device.DeviceState.StateChanged += OnPropertyChanged;
			}
			_presenterItem = presenterItem;
			_presenterItem.IsPoint = true;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
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
			if (Device == null)
				return;

			if (_tooltip == null)
			{
				_tooltip = new ImageTextStateTooltipViewModel();
				_tooltip.TitleViewModel.Title = string.Format("{0} - {1}", Device.PresentationAddressAndDriver, Device.Driver.ShortName).TrimEnd();
				_tooltip.TitleViewModel.ImageSource = Device.Driver.ImageSource;
			}
			_tooltip.StateViewModel.Title = Device.DeviceState.StateClass.ToDescription();
			_tooltip.StateViewModel.ImageSource = Device.DeviceState.StateClass.ToIconSource();
			_tooltip.Update();
		}

		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
		protected override Brush GetBrush()
		{
			return DevicePictureCache.GetDynamicXBrush(Device);
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		private void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Device.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Device = Device
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Device.UID);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree);
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = Helper.SetHeader("Показать в дереве", "pack://application:,,,/Controls;component/Images/BTree.png"),
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = Helper.SetHeader("Показать связанные события", "pack://application:,,,/Controls;component/Images/BJournal.png"),
					Command = ShowJournalCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = Helper.SetHeader("Свойства", "pack://application:,,,/Controls;component/Images/BSettings.png"),
					Command = ShowPropertiesCommand
				});
			}
			return _contextMenu;
		}
	}
}