using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Common.GK;

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
			SendManager.Send(binaryBase.GkDatabaseParent, 3, 13, 0, bytes);
		}
	}
}