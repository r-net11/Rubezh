using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class PropertiesViewModel : BaseViewModel
	{
		public GKDevice Device { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<ShortPropertyViewModel> ShortProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }
		public bool HasAUParameters { get; private set; }

		public PropertiesViewModel(GKDevice device)
		{
			Device = device;
			Update();
		}
		public void Update()
		{
			StringProperties = new List<StringPropertyViewModel>();
			ShortProperties = new List<ShortPropertyViewModel>();
			BoolProperties = new List<BoolPropertyViewModel>();
			EnumProperties = new List<EnumPropertyViewModel>();
			if (Device != null)
			{
				if (Device.PredefinedName == "Тест")
				{
					return;
				}
				foreach (var driverProperty in Device.Driver.Properties)
				{
					if (driverProperty.CanNotEdit)
						continue;

					switch (driverProperty.DriverPropertyType)
					{
						case GKDriverPropertyTypeEnum.StringType:
							StringProperties.Add(new StringPropertyViewModel(driverProperty, Device));
							break;
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

					HasAUParameters = true;
				}
			}

			OnPropertyChanged(() => StringProperties);
			OnPropertyChanged(() => ShortProperties);
			OnPropertyChanged(() => BoolProperties);
			OnPropertyChanged(() => EnumProperties);
		}

		public bool HasParameters
		{
			get
			{
				if ((Device == null) || (Device.PredefinedName == "Тест"))
					return false;
				return Device.Properties.Count != 0;
			}
		}
	}
}