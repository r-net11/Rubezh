using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected SKDDevice Device;
		protected XDriverProperty DriverProperty;

		public BasePropertyViewModel(XDriverProperty driverProperty, SKDDevice device)
		{
			DriverProperty = driverProperty;
			IsAUParameter = driverProperty.IsAUParameter;
			Device = device;

			if (!Device.Properties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default, false);
			}
		}

		public bool IsAUParameter { get; set; }
		public string Caption
		{
			get { return DriverProperty.Caption; }
		}

		public string ToolTip
		{
			get { return DriverProperty.ToolTip; }
		}

		public bool IsEnabled
		{
			get { return !DriverProperty.IsReadOnly; }
		}

		protected void Save(ushort value, bool useSaveService = true)
		{
			if (useSaveService)
			{
				ServiceFactory.SaveService.SKDChanged = true;
			}

			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.Value = value;
			}
			else
			{
				var newProperty = new XProperty()
				{
					Name = DriverProperty.Name,
					Value = value,
				};
				Device.Properties.Add(newProperty);
			}
			Device.OnChanged();
		}
	}
}