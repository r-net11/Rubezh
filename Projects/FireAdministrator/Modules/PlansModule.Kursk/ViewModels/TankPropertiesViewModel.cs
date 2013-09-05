using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using System.Collections.ObjectModel;
using FiresecClient;
using XFiresecAPI;
using PlansModule.Kursk.Designer;

namespace PlansModule.Kursk.ViewModels
{
	public class TankPropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementRectangleTank _element;

		public TankPropertiesViewModel(ElementRectangleTank element)
		{
			_element = element;
			Title = "Свойства фигуры: Бак";

			XDevices = new ObservableCollection<XDevice>(XManager.DeviceConfiguration.Devices.Where(item => item.Driver.DriverType == XDriverType.RSR2_Bush));
			if (_element.XDeviceUID != Guid.Empty)
				SelectedXDevice = XDevices.FirstOrDefault(x => x.UID == _element.XDeviceUID);
		}

		public ObservableCollection<XDevice> XDevices { get; private set; }

		private XDevice _selectedXDevice;
		public XDevice SelectedXDevice
		{
			get { return _selectedXDevice; }
			set
			{
				_selectedXDevice = value;
				OnPropertyChanged(() => SelectedXDevice);
			}
		}

		protected override bool Save()
		{
			Helper.SetXDevice(_element, SelectedXDevice);
			return base.Save();
		}
	}
}
