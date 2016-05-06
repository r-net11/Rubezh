using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.SKD;

namespace StrazhModule.ViewModels
{
	public class BoolPropertyViewModel : BasePropertyViewModel
	{
		public BoolPropertyViewModel(SKDDriverProperty driverProperty, SKDDevice device)
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
				OnPropertyChanged(()=>IsChecked);
				Save(value ? (ushort)1 : (ushort)0);
			}
		}
	}
}