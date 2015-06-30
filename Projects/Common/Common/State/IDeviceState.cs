using System;
using FiresecAPI.GK;

namespace Common
{
	public interface IDeviceState
	{
		XStateClass StateClass { get; }
		string Name { get; }
		event Action StateChanged;
	}
}