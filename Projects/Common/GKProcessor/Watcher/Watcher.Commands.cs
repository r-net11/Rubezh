using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public partial class Watcher
	{
		public static void SendControlCommand(GKBase gkBase, GKStateBit stateBit, string description)
		{
			var code = 0x80 + (int)stateBit;
			SendControlCommand(gkBase, (byte)code, description);
		}

		public static void SendControlCommand(GKBase gkBase, byte code, string description)
		{
			var bytes = new List<byte>();
			var databaseNo = gkBase.GKDescriptorNo;
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);
			if (gkBase.GkDatabaseParent != null)
			{
				WatcherManager.Send(new Action<SendResult>(sendResult =>
					{
						if (sendResult.HasError)
						{
							GKProcessorManager.AddGKMessage(JournalEventNameType.Ошибка_при_выполнении_команды, JournalEventDescriptionType.NULL, description, gkBase, null);
						}
					}),
					gkBase.GkDatabaseParent, 3, 13, 0, bytes);
			}
		}

		public static void SendControlCommandMRO(GKBase gkBase, byte code, byte code2)
		{
			var bytes = new List<byte>();
			var databaseNo = gkBase.GKDescriptorNo;
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);
			bytes.Add(code2);
			if (gkBase.GkDatabaseParent != null)
			{
				WatcherManager.Send(OnCompleted, gkBase.GkDatabaseParent, 3, 13, 0, bytes);
			}
		}

		static void OnCompleted(SendResult sendResult)
		{
			if (sendResult.HasError)
			{
				GKProcessorManager.AddGKMessage(JournalEventNameType.Ошибка_при_выполнении_команды, JournalEventDescriptionType.NULL, "", null, null);
			}
		}
	}
}