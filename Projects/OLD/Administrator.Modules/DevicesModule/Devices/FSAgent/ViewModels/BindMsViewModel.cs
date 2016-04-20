using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class BindMsViewModel : SaveCancelDialogViewModel
	{
		Device _device;

		public BindMsViewModel(Device device, List<string> serials)
		{
			_device = device;
			Serials = serials;
			Title = "Выбор серийного номера для устройства";
		}

		public string CurrentSerial
		{
			get
			{
				string serialNo = "";
				var serialNoProperty = _device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (serialNoProperty != null)
					serialNo = serialNoProperty.Value;

				return serialNo;
			}
		}

		public List<string> Serials { get; set; }

		string _selectedSerial;
		public string SelectedSerial
		{
			get { return _selectedSerial; }
			set
			{
				_selectedSerial = value;
				OnPropertyChanged(() => SelectedSerial);
			}
		}

		void SetSerialNo(string serialNo)
		{
			var serialNoProperty = _device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
			if (serialNoProperty == null)
			{
				serialNoProperty = new Property() { Name = "SerialNo" };
				_device.Properties.Add(serialNoProperty);
			}
			serialNoProperty.Value = serialNo;
		}

		protected override bool Save()
		{
			if (SelectedSerial != null)
				SetSerialNo(SelectedSerial);
			return base.Save();
		}
	}
}