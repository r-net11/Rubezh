using System.Linq;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class BoolPropertyViewModel : BasePropertyViewModel
	{
		public BoolPropertyViewModel(GKDriverProperty driverProperty, GKDevice device)
			: base(driverProperty, device)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
			if (property != null)
				IsChecked = property.Value > 0;
			else
				IsChecked = (driverProperty.Default == (ushort)1) ? true : false;
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
	}
}