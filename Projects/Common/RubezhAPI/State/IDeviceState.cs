using RubezhAPI.GK;
using System;

namespace RubezhAPI
{
	public interface IDeviceState
	{
		XStateClass StateClass { get; }
		string Name { get; }
		event Action StateChanged;
	}
}