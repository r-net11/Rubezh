using FiresecAPI.GK;
using Graybox.OPC.ServerToolkit.CLRWrapper;

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