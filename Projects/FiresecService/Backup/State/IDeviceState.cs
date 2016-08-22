using FiresecAPI.GK;
using System;

namespace Common
{
	public interface IDeviceState
	{
		XStateClass StateClass { get; }

		string Name { get; }

		event Action StateChanged;
	}
}