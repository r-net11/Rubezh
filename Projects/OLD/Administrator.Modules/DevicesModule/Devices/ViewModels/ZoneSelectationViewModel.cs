using System.ComponentModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
	public class ZoneSelectationViewModel : SaveCancelDialogViewModel
	{
		Device Device;

		public ZoneSelectationViewModel(Device device)
		{
			Title = "Выбор зоны устройства " + device.PresentationAddressAndName;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Device = device;
			IsGuardDevice = (device.Driver.DeviceType == DeviceType.Sequrity);

			Zones = new BindingList<ZoneViewModel>();
			foreach (var zone in from zone in FiresecManager.Zones orderby zone.No select zone)
			{
				var isGuardZone = (zone.ZoneType == ZoneType.Guard);
				if (isGuardZone ^ IsGuardDevice)
					continue;

				if (device.Driver.DriverType == DriverType.StopButton || device.Driver.DriverType == DriverType.StartButton || device.Driver.DriverType == DriverType.AutomaticButton)
				{
					if (!zone.DevicesInZone.Any(x => x.Driver.DriverType == DriverType.MPT))
						continue;
				}

				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			if (Device.Zone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone == Device.Zone);
		}

		public bool IsGuardDevice { get; private set; }

		public BindingList<ZoneViewModel> Zones { get; private set; }

		private ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			var createZoneEventArg = new CreateZoneEventArg();
			createZoneEventArg.Zone = new Zone();
			if (IsGuardDevice)
				createZoneEventArg.Zone.ZoneType = ZoneType.Guard;
			else
				createZoneEventArg.Zone.ZoneType = ZoneType.Fire;
			ServiceFactory.Events.GetEvent<CreateZoneEvent>().Publish(createZoneEventArg);
			if (!createZoneEventArg.Cancel)
			{
				var zoneViewModel = new ZoneViewModel(createZoneEventArg.Zone);
				Zones.Add(zoneViewModel);
				SelectedZone = zoneViewModel;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update(SelectedZone.Zone);
			OnPropertyChanged(() => Zones);
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool CanSave()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			FiresecManager.FiresecConfiguration.AddDeviceToZone(Device, SelectedZone.Zone);
			return base.Save();
		}
	}
}