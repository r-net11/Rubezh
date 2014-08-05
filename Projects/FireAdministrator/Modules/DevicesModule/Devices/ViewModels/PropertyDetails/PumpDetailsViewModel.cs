using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class PumpDetailsViewModel : SaveCancelDialogViewModel
	{
		Device _device;

		public PumpDetailsViewModel(Device device)
		{
			Title = "Параметры устройства: Насос";

			_device = device;

			var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "Time");
			if ((timeoutProperty == null) || (timeoutProperty.Value == null))
				Timeout = 0;
			else
				Timeout = int.Parse(timeoutProperty.Value);
		}

		public string PropertyName
		{
			get
			{
				switch (_device.Driver.DriverType)
				{
					case DriverType.Pump:
						return "Время выхода на режим, с";

					case DriverType.JokeyPump:
						return "Время восстановления давления, мин";

					case DriverType.Compressor:
						return "Время восстановления давления, мин";

					case DriverType.CompensationPump:
						return "Время аварии пневмоемкости, мин";
				}
				return "";
			}
		}

		int _timeout;
		public int Timeout
		{
			get { return _timeout; }
			set
			{
				_timeout = value;
				OnPropertyChanged(() => Timeout);
			}
		}

		protected override bool Save()
		{
			var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "Time");
			if (timeoutProperty == null)
			{
				timeoutProperty = new Property() { Name = "Time" };
				_device.Properties.Add(timeoutProperty);
			}
			timeoutProperty.Value = Timeout.ToString();
			return base.Save();
		}
	}
}