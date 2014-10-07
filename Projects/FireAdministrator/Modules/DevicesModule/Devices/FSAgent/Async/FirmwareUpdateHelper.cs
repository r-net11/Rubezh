using System.IO;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Microsoft.Win32;

namespace DevicesModule.ViewModels
{
	public static class FirmwareUpdateHelper
	{
		static Device _device;
		static bool _isUsb;
		static string _fileName;
		static OperationResult<string> _operationResult_1;
		static OperationResult<string> _operationResult_2;

		public static void Run(Device device, bool isUsb)
		{
			_device = device;
			_isUsb = isUsb;

			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.HXP)|*.HXP|All files (*.*)|*.*";

			if (openFileDialog.ShowDialog() == true)
			{
				_fileName = openFileDialog.FileName;
				ServiceFactory.ProgressService.Run(OnVarifyPropgress, OnVerifyCompleted, _device.PresentationAddressAndName + ". Обновление прошивки");
			}
		}

		static void OnVarifyPropgress()
		{
			_operationResult_1 = FiresecManager.DeviceVerifyFirmwareVersion(_device, _isUsb, new FileInfo(_fileName).FullName);
		}

		static void OnVerifyCompleted()
		{
			if (_operationResult_1.HasError)
			{
				MessageBoxService.ShowErrorExtended(_operationResult_1.Error, "Ошибка при выполнении операции");
				return;
			}
			else
			{
				if (MessageBoxService.ShowQuestionYesNo(_operationResult_1.Result))
				{
					ServiceFactory.ProgressService.Run(OnUpdatePropgress, OnUpdateCompleted, _device.PresentationAddressAndName + ". Обновление прошивки");
				}
			}
		}

		static void OnUpdatePropgress()
		{
			_operationResult_2 = FiresecManager.DeviceUpdateFirmware(_device, _isUsb, new FileInfo(_fileName).FullName);
		}

		static void OnUpdateCompleted()
		{
			if (_operationResult_2.HasError)
			{
				MessageBoxService.ShowErrorExtended(_operationResult_2.Error, "Ошибка при выполнении операции");
				return;
			}
			if (string.IsNullOrEmpty(_operationResult_2.Result))
				_operationResult_2.Result = "Операция завершилась успешно";
			MessageBoxService.ShowExtended(_operationResult_2.Result);
		}
	}
}