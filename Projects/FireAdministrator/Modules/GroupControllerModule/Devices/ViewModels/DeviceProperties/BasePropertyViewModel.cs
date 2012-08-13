using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected XDevice _xDevice;
		protected XDriverProperty _xDriverProperty;

		public BasePropertyViewModel(XDriverProperty xDriverProperty, XDevice xDevice)
		{
			_xDriverProperty = xDriverProperty;
			_xDevice = xDevice;
		}

		public string Caption
		{
			get { return _xDriverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return _xDriverProperty.ToolTip; }
		}

		protected void Save(ushort value)
		{
			ServiceFactory.SaveService.XDevicesChanged = true;

			var property = _xDevice.Properties.FirstOrDefault(x => x.Name == _xDriverProperty.Name);
			if (property == null)
			{
				property = new XProperty()
				{
					Name = _xDriverProperty.Name
				};
				_xDevice.Properties.Add(property);
			}
			property.Value = value;
		}

		protected void SaveStringValue(string value)
		{
			var property = _xDevice.Properties.FirstOrDefault(x => x.Name == _xDriverProperty.Name);
			if (property == null)
			{
				property = new XProperty()
				{
					Name = _xDriverProperty.Name
				};
				_xDevice.Properties.Add(property);
			}
			property.StringValue = value;

			ServiceFactory.SaveService.XDevicesChanged = true;
		}
	}
}