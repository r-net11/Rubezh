using System;
using System.Linq;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using System.Collections.Generic;
using System.Threading;

namespace DevicesModule.ViewModels
{
	public class DeviceControlViewModel : BaseViewModel
	{
		Device _device;

		public DeviceControlViewModel(Device device)
		{
			_device = device;
			ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);
			Commands = new List<DriverProperty>();

			var blockNames = new HashSet<string>();

			foreach (var property in device.Driver.Properties)
			{
				if (property.IsControl)
				{
					Commands.Add(property);
					blockNames.Add(property.BlockName);
				}
			}

			if (blockNames.Count > 0)
				BlockName = blockNames.First();
		}

		public string BlockName { get; private set; }

		public List<DriverProperty> Commands { get; private set; }

		DriverProperty _selectedCommand;
		public DriverProperty SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				OnPropertyChanged("SelectedCommand");
			}
		}

		bool CanConfirm()
		{
			return SelectedCommand != null && IsBuisy == false;
		}

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			var thread = new Thread(DoConfirm);
			thread.Start();
		}

		bool IsBuisy = false;

		void DoConfirm()
		{
			IsBuisy = true;
			var result = FiresecManager.FiresecService.ExecuteCommand(_device.UID, SelectedCommand.Name);
			IsBuisy = false;
			OnPropertyChanged("ConfirmCommand");
		}
	}
}