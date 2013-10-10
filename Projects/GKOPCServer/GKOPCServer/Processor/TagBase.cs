using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graybox.OPC.ServerToolkit.CLRWrapper;
using XFiresecAPI;

namespace GKOPCServer
{
	public class TagBase
	{
		public int TagId { get; protected set; }

		protected void ChangedState(XStateClass stateClass)
		{
			GKOPCManager.OPCDAServer.BeginUpdate();
			GKOPCManager.OPCDAServer.UpdateTags(new int[1] { TagId }, new object[1] { stateClass }, Quality.Good, FileTime.UtcNow, ErrorCodes.Ok, false);
			GKOPCManager.OPCDAServer.EndUpdate(false);
		}
	}
}