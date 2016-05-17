using System.Linq;
using RubezhAPI.GK;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class StringPropertyViewModel : BasePropertyViewModel
	{
		public StringPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
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
				Save(value);
				OnPropertyChanged(() => Text);
			}
		}

		protected void Save(string value, bool useSaveService = true)
		{
			if (useSaveService)
			{
				ServiceFactory.SaveService.GKChanged = true;
			}

			var systemProperty = Device.Properties.FirstOrDefault(x => x.Name == DriverProperty.Name);
			if (systemProperty != null)
			{
				systemProperty.Name = DriverProperty.Name;
				systemProperty.StringValue = value;
			}
			else
			{
				var newProperty = new GKProperty()
				{
					Name = DriverProperty.Name,
					StringValue = value
				};
				Device.Properties.Add(newProperty);
			}
			UpdateDeviceParameterMissmatchType();
			Device.OnChanged();
			Device.OnAUParametersChanged();
		}
	}
}