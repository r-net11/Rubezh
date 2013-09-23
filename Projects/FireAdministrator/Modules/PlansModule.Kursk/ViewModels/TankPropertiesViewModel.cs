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

			Devices = new ObservableCollection<XDevice>(XManager.Devices.Where(item => item.Driver.DriverType == XDriverType.RSR2_Bush));
			if (_element.XDeviceUID != Guid.Empty)
				SelectedDevice = Devices.FirstOrDefault(x => x.UID == _element.XDeviceUID);
		}

		public ObservableCollection<XDevice> Devices { get; private set; }

		private XDevice _selectedDevice;
		public XDevice SelectedDevice
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
			Helper.SetXDevice(_element, SelectedDevice);
			return base.Save();
		}
	}
}