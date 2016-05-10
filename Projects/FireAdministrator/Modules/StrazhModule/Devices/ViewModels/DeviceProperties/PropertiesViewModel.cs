using System.Collections.Generic;
using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class PropertiesViewModel : BaseViewModel
	{
		public SKDDevice Device { get; private set; }
		public List<StringPropertyViewModel> StringProperties { get; set; }
		public List<ShortPropertyViewModel> ShortProperties { get; set; }
		public List<BoolPropertyViewModel> BoolProperties { get; set; }
		public List<EnumPropertyViewModel> EnumProperties { get; set; }
		public bool HasAUParameters { get; private set; }

		public PropertiesViewModel(SKDDevice device)
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
				var uiVisibleProperties = new[] { "address", "port", "login", "password" };
				foreach (var driverProperty in Device.Driver.Properties)
				{
					var property = Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
					if (property == null)
					{
						property = new SKDProperty();
						property.DriverProperty = driverProperty;
						property.Name = driverProperty.Name;
						property.Value = driverProperty.Default;
						property.StringValue = driverProperty.StringDefault;
						Device.Properties.Add(property);
					}

					if (uiVisibleProperties.Any(x => x == driverProperty.Name.ToLower()))
					{
						switch (driverProperty.DriverPropertyType)
						{
							case SKDDriverType.StringType:
								StringProperties.Add(new StringPropertyViewModel(driverProperty, Device));
								break;
							case SKDDriverType.IntType:
								ShortProperties.Add(new ShortPropertyViewModel(driverProperty, Device));
								break;
							case SKDDriverType.BoolType:
								BoolProperties.Add(new BoolPropertyViewModel(driverProperty, Device));
								break;
							case SKDDriverType.EnumType:
								EnumProperties.Add(new EnumPropertyViewModel(driverProperty, Device));
								break;
						}
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
				if (Device == null)
					return false;
				return Device.Properties.Count != 0;
			}
		}
	}
}