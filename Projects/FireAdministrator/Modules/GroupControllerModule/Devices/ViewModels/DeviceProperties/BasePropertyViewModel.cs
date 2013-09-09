using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected XDevice _device;
		protected XDriverProperty _driverProperty;

		public BasePropertyViewModel(XDriverProperty driverProperty, XDevice device)
		{
			_driverProperty = driverProperty;
			_device = device;
		}

		public string Caption
		{
			get { return _driverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return _driverProperty.ToolTip; }
		}

		protected void Save(ushort value)
		{
			ServiceFactory.SaveService.GKChanged = true;

			var property = _device.Properties.FirstOrDefault(x => x.Name == _driverProperty.Name);
			if (property == null)
			{
				property = new XProperty()
				{
					Name = _driverProperty.Name
				};
				_device.Properties.Add(property);
			}
			property.Value = value;
		}

		protected void SaveStringValue(string value)
		{
			var property = _device.Properties.FirstOrDefault(x => x.Name == _driverProperty.Name);
			if (property == null)
			{
				property = new XProperty()
				{
					Name = _driverProperty.Name
				};
				_device.Properties.Add(property);
			}
			property.StringValue = value;

			ServiceFactory.SaveService.GKChanged = true;
		}
	}
}