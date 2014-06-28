using System;
namespace Common
{
	public interface IDeviceState<TStateType>
	{
		TStateType StateType { get; }
		event Action StateChanged;
	}
}