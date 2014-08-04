using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure;
using System.ComponentModel;
using Infrastructure.Events;
using System.Threading;

namespace SKDModule.ViewModels
{
	public class ControllerPropertiesViewModel : DialogViewModel
	{
		public SKDDevice Device { get; private set; }
		public SKDDeviceInfo DeviceInfo { get; private set; }

		public ControllerPropertiesViewModel(SKDDevice device, SKDDeviceInfo deviceInfo)
		{
			Title = "Конфигурация контроллера";
			Device = device;
			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime);
			ResetCommand = new RelayCommand(OnReset);
			RebootCommand = new RelayCommand(OnReboot);
			RewriteAllCardsCommand = new RelayCommand(OnRewriteAllCards);

			DeviceInfo = deviceInfo;
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = FiresecManager.FiresecService.SKDSyncronyseTime(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция синхронизации времени завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции синхронизации времени", result.Error);
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			var result = FiresecManager.FiresecService.SKDResetController(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции", result.Error);
			}
		}

		public RelayCommand RebootCommand { get; private set; }
		void OnReboot()
		{
			var result = FiresecManager.FiresecService.SKDRebootController(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции", result.Error);
			}
		}

		public RelayCommand RewriteAllCardsCommand { get; private set; }
		void OnRewriteAllCards()
		{
			var thread = new Thread(() =>
			{
				var result = FiresecManager.FiresecService.SKDRewriteAllCards(Device);

				ApplicationService.Invoke(new Action(() =>
				{
					if (result.HasError)
					{
						LoadingService.Close();
						MessageBoxService.ShowWarning(result.Error);
					}
				}));
			});
			thread.Name = "DeviceCommandsViewModel OnWriteTimeSheduleConfiguration";
			thread.Start();
		}
	}
}