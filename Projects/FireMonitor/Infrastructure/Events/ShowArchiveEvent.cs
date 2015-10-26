using Microsoft.Practices.Prism.Events;
using RubezhAPI.SKD;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class ShowArchiveEvent : CompositePresentationEvent<List<Guid>>
	{
	}
}