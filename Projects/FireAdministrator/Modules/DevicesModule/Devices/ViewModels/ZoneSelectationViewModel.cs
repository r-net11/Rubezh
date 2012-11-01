using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using Infrastructure.Events;
using Infrastructure;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class ZoneSelectationViewModel : SaveCancelDialogViewModel
	{
		Device Device;

        public ZoneSelectationViewModel(Device device)
		{
            Title = "Выбор зоны устройства " + device.PresentationAddressAndDriver;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

            Device = device;
            Zones = new ObservableCollection<ZoneViewModel>();
            foreach (var zone in from zone in FiresecManager.Zones orderby zone.No select zone)
            {
                var zoneViewModel = new ZoneViewModel(zone);
                Zones.Add(zoneViewModel);
            }
            if (Device.Zone != null)
                SelectedZone = Zones.FirstOrDefault(x=>x.Zone == Device.Zone);
		}

        public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		private ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			var createZoneEventArg = new CreateZoneEventArg();
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
			OnPropertyChanged("Zones");
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
