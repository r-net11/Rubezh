using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;
using RubezhClient;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class GKReaderSelectationViewModel : SaveCancelDialogViewModel
	{
		public GKReaderSelectationViewModel(Guid deviceUID)
		{
			Title = "Выбор считывателя";
			Devices = new ObservableCollection<GKDevice>();
			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.RSR2_CardReader)
					Devices.Add(device);
			}
			SelectedDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
		}

		public ObservableCollection<GKDevice> Devices { get; private set; }

		GKDevice _selectedDevice;
		public GKDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		protected override bool Save()
		{
			ClientSettings.SKDSettings.CardCreatorReaderUID = SelectedDevice != null ? SelectedDevice.UID : Guid.Empty;
			return base.Save();
		}
	}
}