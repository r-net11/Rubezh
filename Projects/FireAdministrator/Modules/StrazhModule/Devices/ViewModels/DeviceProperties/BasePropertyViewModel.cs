using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected SKDDevice Device;
		protected SKDDriverProperty DriverProperty;

		public BasePropertyViewModel(SKDDriverProperty driverProperty, SKDDevice device)
		{
			DriverProperty = driverProperty;
			Device = device;

			if (!Device.Properties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default, false);
			}
		}

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
			get { return true; }
		}

		protected void Save(int value, bool useSaveService = true)
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
				var newProperty = new SKDProperty()
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