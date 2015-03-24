using System;
namespace Common
{
	public interface IDeviceState<TStateType>
	{
		TStateType StateType { get; }
		string StateTypeName { get; }
		event Action StateChanged;
	}
}