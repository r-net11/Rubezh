using Common;

namespace StrazhAPI.GK
{
	public interface IStateProvider : IIdentity
	{
		IDeviceState StateClass { get; }
	}
}