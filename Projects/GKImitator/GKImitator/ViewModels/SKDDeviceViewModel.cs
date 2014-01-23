using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using GKImitator.Processor;
using Infrastructure.Common;

namespace GKImitator.ViewModels
{
	public class SKDDeviceViewModel : BaseViewModel
	{
		public SKDDevice SKDDevice { get; private set; }
		SKDImitatorProcessor SKDImitatorProcessor;

		public SKDDeviceViewModel(SKDDevice device)
		{
			CardCommand = new RelayCommand(OnCard);
			SKDDevice = device;
			var portProperty = device.Properties.FirstOrDefault(x => x.Name == "Port");
			if (portProperty != null)
			{
				var portNo = portProperty.Value;
				SKDImitatorProcessor = new SKDImitatorProcessor(portNo);
				SKDImitatorProcessor.Start();
			}
		}

		public RelayCommand CardCommand { get; private set; }
		void OnCard()
		{

		}
	}
}