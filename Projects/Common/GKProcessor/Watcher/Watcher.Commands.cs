using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Services;

namespace GKProcessor
{
	public partial class Watcher
	{
        public static bool SendControlCommand(XBase xBase, XStateBit stateType, bool mustValidatePassword = true)
        {
            var code = 0x80 + (int)stateType;
            return SendControlCommand(xBase, (byte)code, mustValidatePassword);
        }

        public static bool SendControlCommand(XBase xBase, byte code, bool mustValidatePassword = true)
        {
            var bytes = new List<byte>();
            var databaseNo = xBase.GKDescriptorNo;
            bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
            bytes.Add(code);

            var result = true;
            if (mustValidatePassword)
            {
                result = ServiceFactoryBase.SecurityService != null && ServiceFactoryBase.SecurityService.Validate();
            }
            if (result)
            {
                WatcherManager.Send(OnCompleted, SendPriority.Normal, xBase.GkDatabaseParent, 3, 13, 0, bytes);
            }
            return result;
        }

        public static void SendControlCommandMRO(XBase xBase, byte code, byte code2)
        {
            var bytes = new List<byte>();
            var databaseNo = xBase.GKDescriptorNo;
            bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
            bytes.Add(code);
            bytes.Add(code2);

            WatcherManager.Send(OnCompleted, SendPriority.Normal, xBase.GkDatabaseParent, 3, 13, 0, bytes);
        }

        static void OnCompleted(SendResult sendResult)
        {
            if (sendResult.HasError)
            {
                ApplicationService.BeginInvoke(() =>
                {
                    MessageBoxService.ShowError("Ошибка при выполнении операции");
                });
            }
        }
	}
}