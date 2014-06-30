using System;
using Common;

namespace Infrustructure.Plans.Interfaces
{
	public interface IChangedNotification : IIdentity
	{
		event Action Changed;
	}
}
