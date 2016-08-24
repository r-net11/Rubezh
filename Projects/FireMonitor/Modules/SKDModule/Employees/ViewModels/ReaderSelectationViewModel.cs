using System;
using System.Collections.ObjectModel;
using System.Linq;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ReaderSelectationViewModel : SaveCancelDialogViewModel
	{
		public ReaderSelectationViewModel(Guid deviceUID)
		{
			Title = CommonViewModels.SelectReader;
			Devices = new ObservableCollection<SKDDevice>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.DriverType == SKDDriverType.Reader)
					Devices.Add(device);
			}
			SelectedDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
		}

		public ObservableCollection<SKDDevice> Devices { get; private set; }

		SKDDevice _selectedDevice;
		public SKDDevice SelectedDevice
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