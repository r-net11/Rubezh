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

			Devices = new ObservableCollection<GKDevice>(GKManager.Devices.Where(item => item.DriverType == GKDriverType.RSR2_Bush));
			if (_element.DeviceUID != Guid.Empty)
				SelectedDevice = Devices.FirstOrDefault(x => x.UID == _element.DeviceUID);
		}

		public ObservableCollection<GKDevice> Devices { get; private set; }

		private GKDevice _selectedDevice;
		public GKDevice SelectedDevice
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
			Helper.SetGKDevice(_element, SelectedDevice);
			return base.Save();
		}
	}
}