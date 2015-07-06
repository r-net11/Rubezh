using Common;
using System;

namespace Infrustructure.Plans.Interfaces
{
	public interface IChangedNotification : IIdentity
	{
		event Action Changed;
	}
}