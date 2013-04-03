using System.Collections.Generic;
using Common.GK;
using XFiresecAPI;
using Infrastructure.Common.Windows;

namespace GKModule
{
	public static class ObjectCommandSendHelper
	{
		public static void SendControlCommand(XBinaryBase binaryBase, XStateType stateType)
		{
			var code = 0x80 + (int)stateType;
			SendControlCommand(binaryBase, (byte)code);
		}

		public static void SendControlCommand(XBinaryBase binaryBase, byte code)
		{
			var bytes = new List<byte>();
			var databaseNo = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);
			WatcherManager.Send(OnCompleted, SendPriority.Normal, binaryBase.GkDatabaseParent, 3, 13, 0, bytes);
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