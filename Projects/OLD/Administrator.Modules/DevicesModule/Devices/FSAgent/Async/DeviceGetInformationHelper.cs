using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.Windows;

namespace DevicesModule.ViewModels
{
	public static class DeviceGetInformationHelper
	{
		static Device Device;
		static bool IsUsb;
		static OperationResult<string> _operationResult;

		public static void Run(Device device, bool isUsb)
		{
			Device = device;
			IsUsb = isUsb;
			ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, Device.PresentationAddressAndName + ". Чтение информации об устройстве");
		}

		static void OnPropgress()
		{
			_operationResult = FiresecManager.DeviceGetInformation(Device, IsUsb);
		}

		static void OnCompleted()
		{
			if (_operationResult.HasError)
			{
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			DialogService.ShowModalWindow(new DeviceDescriptionViewModel(Device, _operationResult.Result));
		}
	}
}