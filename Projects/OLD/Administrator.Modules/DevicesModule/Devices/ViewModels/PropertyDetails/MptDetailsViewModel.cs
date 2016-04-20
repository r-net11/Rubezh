using System;
using System.Linq;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class MptDetailsViewModel : SaveCancelDialogViewModel
	{
		Device _device;

		public MptDetailsViewModel(Device device)
		{
			Title = "Параметры устройства: Модуль пожаротушения";
			_device = device;

			int timeout = GetMPTTimeout(_device);
			TimeoutMinutes = timeout / 60;
			TimeoutSeconds = timeout % 60;

			var actionProperty = _device.Properties.FirstOrDefault(x => x.Name == "Config");
			if ((actionProperty == null) || (actionProperty.Value == null))
				IsAutoBlock = false;
			else
				IsAutoBlock = true;
		}

		int _timeoutMinutes;
		public int TimeoutMinutes
		{
			get { return _timeoutMinutes; }
			set
			{
				_timeoutMinutes = value;
				OnPropertyChanged(() => TimeoutMinutes);
			}
		}

		int _timeoutSeconds;
		public int TimeoutSeconds
		{
			get { return _timeoutSeconds; }
			set
			{
				_timeoutSeconds = value;
				OnPropertyChanged(() => TimeoutSeconds);
			}
		}

		bool _isAutoBlock;
		public bool IsAutoBlock
		{
			get { return _isAutoBlock; }
			set
			{
				_isAutoBlock = value;
				OnPropertyChanged(() => IsAutoBlock);
			}
		}

		protected override bool Save()
		{
			var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "RunDelay");
			if (timeoutProperty == null)
			{
				timeoutProperty = new Property() { Name = "RunDelay" };
				_device.Properties.Add(timeoutProperty);
			}
			timeoutProperty.Value = (TimeoutMinutes * 60 + TimeoutSeconds).ToString();

			var actionProperty = _device.Properties.FirstOrDefault(x => x.Name == "Config");
			if (actionProperty == null)
			{
				actionProperty = new Property()
				{
					Name = "Config",
					Value = "1"
				};
				_device.Properties.Add(actionProperty);
			}
			if (IsAutoBlock == false)
			{
				_device.Properties.Remove(actionProperty);
			}
			return base.Save();
		}

		int GetMPTTimeout(Device device)
		{
			int result = 0;
			try
			{
				var timeoutProperty = device.Properties.FirstOrDefault(x => x.Name == "RunDelay");
				if ((timeoutProperty == null) || (timeoutProperty.Value == null))
					result = 0;
				else
					result = int.Parse(timeoutProperty.Value);
			}
			catch (Exception e)
			{
				Logger.Error(e, "MptDetailsViewModel.GetMPTTimeout");
			}
			return result;
		}
	}
}