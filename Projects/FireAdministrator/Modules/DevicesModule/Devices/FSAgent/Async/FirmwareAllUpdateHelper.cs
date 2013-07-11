using System.IO;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Microsoft.Win32;
using System.Windows.Documents;
using System.Collections.Generic;

namespace DevicesModule.ViewModels
{
    public static class FirmwareAllUpdateHelper
    {
        static Device Device;
        static string FileName;
        static string ReportString;
        static OperationResult<string> OperationResult_1;
        static OperationResult<string> OperationResult_2;
        static List<DriverType> DriverTypesToUpdate;
        
        public static bool IsManyDevicesToUpdate(Device device)
        {
            Device = device;
            GetDriverTypesToUpdate();
            return FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.Where(x => DriverTypesToUpdate.Contains(x.Driver.DriverType)).Count() > 1;
        }

        public static void Run(Device device)
        {
            Device = device;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.HXP)|*.HXP|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
                ServiceFactory.ProgressService.Run(OnUpdateProgress, OnUpdateCompleted, Device.PresentationAddressAndName + ". Обновление прошивки");
            }
        }

        static void OnUpdateProgress()
        {
			bool? isRewriteSame = null;
			bool? isRewriteOld = null;
			bool isUpdateable = true;
			GetDriverTypesToUpdate();
            foreach (var device in FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.Where(x => DriverTypesToUpdate.Contains(x.Driver.DriverType)))
            {
                OperationResult_1 = FiresecManager.DeviceVerifyFirmwareVersion(device, device.IsUsb, new FileInfo(FileName).FullName);
                if (OperationResult_1.HasError)
                {
                    ReportString += "Ошибка при проверке версии " + device.PresentationAddressAndName + " " +OperationResult_1.Error + "\n";
					isUpdateable = false;
                }
				else if (OperationResult_1.Result == "В приборе уже установлена данная версия программного обеспечения. Продолжить ?")
 				{
					switch (isRewriteSame)
					{
						case (true):
							break;
						case (false):
							isUpdateable = false;
							break;
						default:
							isRewriteSame = MessageBoxService.ShowQuestion(OperationResult_1.Result) == MessageBoxResult.Yes;
							break;
					}
				}
				else if (OperationResult_1.Result == "В приборе содержится более новая версия программного обеспечения. Вы уверены что хотите вернуться к старой версии ПО ?")
 				{
					switch (isRewriteOld)
					{
						case (true):
							break;
						case (false):
							isUpdateable = false;
							break;
						default:
							isRewriteSame = MessageBoxService.ShowQuestion(OperationResult_1.Result) == MessageBoxResult.Yes;
							break;
					}
				}
				
				if(isUpdateable)
                {
                    OperationResult_2 = FiresecManager.DeviceUpdateFirmware(device, device.IsUsb, new FileInfo(FileName).FullName);
                    if (OperationResult_2.HasError)
                        ReportString += "Ошибка при выполнении операции " + device.PresentationAddressAndName + " " + OperationResult_2.Error + "\n";
                    else
                        ReportString += "Операция завершилась успешно " + device.PresentationAddressAndName + "\n";
                }
            }
            
        }

        static void GetDriverTypesToUpdate()
        {
            DriverTypesToUpdate = new List<DriverType>();
            var driverType = Device.Driver.DriverType;
            DriverTypesToUpdate.Add(Device.Driver.DriverType);
            if (driverType == DriverType.BUNS)
                DriverTypesToUpdate.Add(DriverType.USB_BUNS);
            if (driverType == DriverType.Rubezh_2AM)
                DriverTypesToUpdate.Add(DriverType.USB_Rubezh_2AM);
            if (driverType == DriverType.Rubezh_2OP)
                DriverTypesToUpdate.Add(DriverType.USB_Rubezh_2OP);
            if (driverType == DriverType.Rubezh_4A)
                DriverTypesToUpdate.Add(DriverType.USB_Rubezh_4A);
            if (driverType == DriverType.Rubezh_P)
                DriverTypesToUpdate.Add(DriverType.USB_Rubezh_P);

            if (driverType == DriverType.USB_BUNS)
                DriverTypesToUpdate.Add(DriverType.BUNS);
            if (driverType == DriverType.USB_Rubezh_2AM)
                DriverTypesToUpdate.Add(DriverType.Rubezh_2AM);
            if (driverType == DriverType.USB_Rubezh_2OP)
                DriverTypesToUpdate.Add(DriverType.Rubezh_2OP);
            if (driverType == DriverType.USB_Rubezh_4A)
                DriverTypesToUpdate.Add(DriverType.Rubezh_4A);
            if (driverType == DriverType.USB_Rubezh_P)
                DriverTypesToUpdate.Add(DriverType.Rubezh_P);
        }

        static void OnUpdateCompleted()
        {
            MessageBoxService.Show(ReportString);
        }

    }
}
