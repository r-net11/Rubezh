using System;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using System.Collections.Generic;

namespace DevicesModule.ViewModels
{
	public class DeviceControlViewModel : BaseViewModel
	{
		Device _device;

		public DeviceControlViewModel(Device device)
		{
			_device = device;
			ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);
			Commands = new List<string>();
			//Commands.Add("Включение");
			//Commands.Add("Включение без задержки");
			//Commands.Add("Выключение");

			foreach (var property in device.Driver.Properties)
			{
				if (property.IsControl)
				{
					Commands.Add(property.Name);
				}
			}
		}

		public List<string> Commands { get; private set; }

		string _selectedCommand;
		public string SelectedCommand
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
			return SelectedCommand != null;
		}

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			FiresecManager.DeviceCustomFunctionExecute(_device.UID, SelectedCommand);
		}
	}
}