using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure;

namespace StrazhModule.ViewModels
{
	public class StringPropertyViewModel : BasePropertyViewModel
	{
		public StringPropertyViewModel(SKDDriverProperty driverProperty, SKDDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			_text = property != null ? property.StringValue : driverProperty.StringDefault;
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
				Save(value);
			}
		}

		protected void Save(string value, bool useSaveService = true)
		{
			if (useSaveService)
			{
				ServiceFactory.SaveService.SKDChanged = true;
			}

			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.StringValue = value;
			}
			else
			{
				var newProperty = new SKDProperty()
				{
					Name = DriverProperty.Name,
					StringValue = value
				};
				Device.Properties.Add(newProperty);
			}
			Device.OnChanged();
		}
	}
}