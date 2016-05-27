using Common;
using System;

namespace StrazhAPI.Plans.Interfaces
{
	public interface IChangedNotification : IIdentity
	{
		event Action Changed;
	}
}