using Common;
namespace RubezhAPI.GK
{
	public interface IStateProvider : IIdentity
	{
		IDeviceState StateClass { get; }
	}
}