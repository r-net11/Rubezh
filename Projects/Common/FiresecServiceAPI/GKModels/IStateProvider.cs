using Common;

namespace FiresecAPI.GK
{
	public interface IStateProvider : IIdentity
	{
		IDeviceState StateClass { get; }
	}
}