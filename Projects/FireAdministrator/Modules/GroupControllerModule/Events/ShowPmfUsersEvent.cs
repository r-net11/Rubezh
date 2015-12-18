using System;
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class GetPmfUsersEvent : CompositePresentationEvent<List<GKUser>>
	{
	}
}