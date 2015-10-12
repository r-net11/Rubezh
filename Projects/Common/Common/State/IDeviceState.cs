using System;
using RubezhAPI.GK;

namespace Common
{
	public interface IDeviceState
	{
		XStateClass StateClass { get; }
		string Name { get; }
		event Action StateChanged;
	}
}