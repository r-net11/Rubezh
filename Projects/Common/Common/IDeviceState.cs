
namespace Common
{
	public interface IDeviceState<TStateType>
	{
		TStateType StateType { get; }
	}
}
