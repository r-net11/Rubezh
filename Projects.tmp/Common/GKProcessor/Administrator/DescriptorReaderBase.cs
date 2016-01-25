using RubezhAPI.GK;
using System;

namespace GKProcessor
{
	public abstract class DescriptorReaderBase
	{
		public abstract bool ReadConfiguration(GKDevice device, Guid clientUID);
		public GKDeviceConfiguration DeviceConfiguration { get; protected set; }
		public string Error { get; protected set; }
	}
}