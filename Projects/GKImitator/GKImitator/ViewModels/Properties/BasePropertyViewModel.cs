using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace GKImitator.ViewModels
{
	public class BasePropertyViewModel : BaseViewModel
	{
		protected GKDevice Device;
		protected GKDriverProperty DriverProperty;

		public BasePropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
		{
			DriverProperty = driverProperty;
			Device = device;

			if (!Device.Properties.Any(x => x.Name == driverProperty.Name))
			{
				Save(driverProperty.Default);
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

		ushort Value;

		protected void Save(ushort value)
		{
			Value = value;
		}

		public void SaveValue()
		{
			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.Value = Value;
			}
			else
			{
				var newProperty = new GKProperty()
				{
					Name = DriverProperty.Name,
					Value = Value,
				};
				Device.Properties.Add(newProperty);
			}
		}
	}
}