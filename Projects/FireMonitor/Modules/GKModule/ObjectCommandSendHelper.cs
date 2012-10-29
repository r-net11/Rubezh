using System.Collections.Generic;
using Common.GK;
using XFiresecAPI;

namespace GKModule
{
	public static class ObjectCommandSendHelper
	{
		public static void SendControlCommand(XBinaryBase binaryBase, byte code)
		{
			var bytes = new List<byte>();
			var databaseNo = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);
            JournalWatcherManager.SafeSendManager.Send(SendPriority.Normal, binaryBase.GkDatabaseParent, 3, 13, 0, bytes);
		}
	}
}