using Infrastructure.Common.Windows;
using Resurs.Processor;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class CommandViewModel
	{
		public Device _device;
		public DeviceCommand Model { get; private set; }

		public CommandViewModel(Device device, DeviceCommand model)
		{
			_device = device;
			Model = model;
		}

		public RelayCommand SendCommand
		{
			get
			{ 
				return new RelayCommand(() => 
					DeviceProcessor.Instance.SendCommand(_device.UID, Model.Name));	
			}
		}
	}
}
