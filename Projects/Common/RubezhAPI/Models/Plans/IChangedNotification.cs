﻿using Common;
using System;

namespace RubezhAPI.Plans.Interfaces
{
	public interface IChangedNotification : IIdentity
	{
		event Action Changed;
		event Action<Guid, Guid> UIDChanged;
	}
}