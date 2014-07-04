using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class LockPropertiesViewModel : DialogViewModel
	{
		public SKDDevice Device { get; private set; }

		public LockPropertiesViewModel(SKDDevice device)
		{
			Title = "Параметры двери";
			Device = device;
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);
			DoorConfiguration = new SKDDoorConfiguration();
		}

		SKDDoorConfiguration _doorConfiguration;
		public SKDDoorConfiguration DoorConfiguration
		{
			get { return _doorConfiguration; }
			set
			{
				_doorConfiguration = value;
				OnPropertyChanged(() => DoorConfiguration);
			}
		}

		public RelayCommand GetDoorConfigurationCommand { get; private set; }
		void OnGetDoorConfiguration()
		{
			var result = FiresecManager.FiresecService.SKDGetDoorConfiguration(Device.UID);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				DoorConfiguration = result.Result;
			}
		}

		public RelayCommand SetDoorConfigurationCommand { get; private set; }
		void OnSetDoorConfiguration()
		{
			var result = FiresecManager.FiresecService.SKDSetDoorConfiguration(Device.UID, DoorConfiguration);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
		}
	}
}