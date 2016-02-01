using RubezhAPI.GK;
using Graybox.OPC.ServerToolkit.CLRWrapper;
using System;

namespace GKOPCServer
{
	public class TagBase

	{
		public GKState State { get; private set; }
		public GKDriverType DriverType { get; set; }
		public Guid UID { get; private set; }

		public TagBase(int tagId, GKState state, Guid uid)
		{
			TagId = tagId;
			State = state;
			UID = uid;
			state.StateChanged += state_StateChanged;
		}

		void state_StateChanged()
		{
			ChangedState(State.StateClass);
		}

		public int TagId { get; private set; }

	    void ChangedState(XStateClass stateClass)
		{
			GKOPCManager.OPCDAServer.BeginUpdate();
			GKOPCManager.OPCDAServer.UpdateTags(new int[1] { TagId }, new object[1] { stateClass }, Quality.Good, FileTime.UtcNow, ErrorCodes.Ok, false);
			GKOPCManager.OPCDAServer.EndUpdate(false);
		}
	}
}