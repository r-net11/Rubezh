using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using ChinaSKDDriverAPI;
using Infrastructure.Common;
using System.Windows;

namespace ControllerSDK.ViewModels
{
	public class DoorConfigurationViewModel : BaseViewModel
	{
		public DoorConfigurationViewModel()
		{
			GetDoorConfigurationCommand = new RelayCommand(OnGetDoorConfiguration);
			SetDoorConfigurationCommand = new RelayCommand(OnSetDoorConfiguration);
		}

		DoorConfiguration _doorConfiguration;
		public DoorConfiguration DoorConfiguration
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
			DoorConfiguration = MainViewModel.Wrapper.GetDoorConfiguration();
			if (DoorConfiguration != null)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand SetDoorConfigurationCommand { get; private set; }
		void OnSetDoorConfiguration()
		{
			DoorConfiguration.ChannelName = "0";
			var result = MainViewModel.Wrapper.SetDoorConfiguration(DoorConfiguration);
			if (result)
			{
				MessageBox.Show("Success");
			}
			else
			{
				MessageBox.Show("Error");
			}
		}
	}
}