using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ControllerSelectationViewModel : SaveCancelDialogViewModel
	{
		public ControllerSelectationViewModel(Guid deviceUID)
		{
			Title = "Выбор контроллера";
			Devices = new ObservableCollection<SKDDevice>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.IsController)
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
			return base.Save();
		}
	}
}