using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKImitator.ViewModels
{
	public class DevicePropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKDevice Device { get; private set; }
		public List<ShortPropertyViewModel> ShortProperties { get; private set; }
		public List<BoolPropertyViewModel> BoolProperties { get; private set; }
		public List<EnumPropertyViewModel> EnumProperties { get; private set; }

		public DevicePropertiesViewModel(GKDevice device)
		{
			Title = "Параметры устройства";
			Device = device;

			ShortProperties = new List<ShortPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();
			if (Device != null)
			{
				if (Device.PredefinedName == "Тест")
				{
					return;
				}
				foreach (var driverProperty in Device.Driver.Properties.Where(x => x.IsAUParameter && !x.CanNotEdit))
				{
					switch (driverProperty.DriverPropertyType)
					{
						case GKDriverPropertyTypeEnum.IntType:
							ShortProperties.Add(new ShortPropertyViewModel(driverProperty, Device));
							break;
						case GKDriverPropertyTypeEnum.BoolType:
							BoolProperties.Add(new BoolPropertyViewModel(driverProperty, Device));
							break;
						case GKDriverPropertyTypeEnum.EnumType:
							EnumProperties.Add(new EnumPropertyViewModel(driverProperty, Device));
							break;
					}
				}
			}
		}

		protected override bool Save()
		{
			foreach (var shortProperty in ShortProperties)
			{
				shortProperty.SaveValue();
			}
			foreach (var boolProperty in BoolProperties)
			{
				boolProperty.SaveValue();
			}
			foreach (var enumProperty in EnumProperties)
			{
				enumProperty.SaveValue();
			}
			return base.Save();
		}
	}
}