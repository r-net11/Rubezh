using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
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

			Devices = new ObservableCollection<XDevice>(XManager.Devices.Where(item => item.DriverType == XDriverType.RSR2_Bush));
			if (_element.XDeviceUID != Guid.Empty)
				SelectedDevice = Devices.FirstOrDefault(x => x.BaseUID == _element.XDeviceUID);
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