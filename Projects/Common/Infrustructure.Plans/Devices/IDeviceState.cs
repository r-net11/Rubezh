
namespace Infrustructure.Plans.Devices
{
	public interface IDeviceState<TStateType>
	{
		TStateType StateType { get; }
	}
}
