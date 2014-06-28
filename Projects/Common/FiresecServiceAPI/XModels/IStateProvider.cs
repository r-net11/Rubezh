using Common;
namespace FiresecAPI.GK
{
	public interface IStateProvider
	{
		IDeviceState<XStateClass> StateClass { get; }
	}
}