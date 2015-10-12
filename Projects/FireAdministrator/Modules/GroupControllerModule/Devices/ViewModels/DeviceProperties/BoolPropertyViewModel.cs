using System.Linq;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class BoolPropertyViewModel : BasePropertyViewModel
	{
		public BoolPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
				_isChecked = property.Value > 0;
			else
				_isChecked = (driverProperty.Default == (ushort)1) ? true : false;
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				Save(value ? (ushort)1 : (ushort)0);
				OnPropertyChanged(() => IsChecked);
			}
		}

		public string DeviceAUParameterValue
		{
			get
			{
				return base.DeviceAUParameterValue == "0" ? "Нет" : (base.DeviceAUParameterValue == "1" ? "Да" : "Неизвестно");
			}
		}
	}
}