using System.IO;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.MessageBox;
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
		static byte[] bytes;

		public static void Run(Device device, bool isUsb)
		{
			_device = device;
			_isUsb = isUsb;

			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.HXP)|*.HXP|All files (*.*)|*.*";

			if (openFileDialog.ShowDialog() == true)
			{
				_fileName = openFileDialog.FileName;
				using (var fileStream = new FileStream(_fileName, FileMode.Open))
				{
					bytes = new byte[fileStream.Length];
					fileStream.Read(bytes, 0, bytes.Length);
				}

				ServiceFactory.ProgressService.Run(OnVarifyPropgress, OnVerifyCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
			}
		}

		static void OnVarifyPropgress()
		{
			_operationResult_1 = FiresecManager.DeviceVerifyFirmwareVersion(_device.UID, _isUsb, bytes, new FileInfo(_fileName).Name);
		}

		static void OnVerifyCompleted()
		{
			if (_operationResult_1.HasError)
			{
				MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult_1.Error);
			}
			else
			{
				if (MessageBoxService.ShowQuestion(_operationResult_1.Result) == MessageBoxResult.Yes)
				{
					ServiceFactory.ProgressService.Run(OnUpdatePropgress, OnUpdateCompleted, _device.PresentationAddressDriver + ". Обновление прошивки");
				}
			}
		}

		static void OnUpdatePropgress()
		{
			_operationResult_2 = FiresecManager.DeviceUpdateFirmware(_device.UID, _isUsb, bytes, new FileInfo(_fileName).Name);
		}

		static void OnUpdateCompleted()
		{
			if (_operationResult_2.HasError)
			{
				MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult_2.Error);
				return;
			}
			if (string.IsNullOrEmpty(_operationResult_2.Result))
				_operationResult_2.Result = "Операция завершилась успешно";
			MessageBoxService.Show(_operationResult_2.Result);
		}
	}
}