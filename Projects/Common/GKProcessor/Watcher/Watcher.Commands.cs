using System;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public partial class Watcher
	{
		public static void SendControlCommand(XBase xBase, XStateBit stateBit, string description)
		{
			var code = 0x80 + (int)stateBit;
			SendControlCommand(xBase, (byte)code, description);
		}

		public static void SendControlCommand(XBase xBase, byte code, string description)
		{
			var bytes = new List<byte>();
			var databaseNo = xBase.GKDescriptorNo;
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);
			if (xBase.GkDatabaseParent != null)
			{
				WatcherManager.Send(new Action<SendResult>(sendResult =>
					{
						if (sendResult.HasError)
						{
							GKProcessorManager.AddGKMessage(EventNameEnum.Ошибка_при_выполнении_команды, description, xBase, null);
						}
					}),
					xBase.GkDatabaseParent, 3, 13, 0, bytes);
			}
		}

		public static void SendControlCommandMRO(XBase xBase, byte code, byte code2)
		{
			var bytes = new List<byte>();
			var databaseNo = xBase.GKDescriptorNo;
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);
			bytes.Add(code2);
			if (xBase.GkDatabaseParent != null)
			{
				WatcherManager.Send(OnCompleted, xBase.GkDatabaseParent, 3, 13, 0, bytes);
			}
		}

		static void OnCompleted(SendResult sendResult)
		{
			if (sendResult.HasError)
			{
				GKProcessorManager.AddGKMessage(EventNameEnum.Ошибка_при_выполнении_команды, "", null, null);
			}
		}
	}
}