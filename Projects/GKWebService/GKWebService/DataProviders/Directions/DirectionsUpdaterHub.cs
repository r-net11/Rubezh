using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;
using GKWebService.Models;
using Microsoft.AspNet.SignalR;

namespace GKWebService.DataProviders
{
	[HubName("directionsUpdater")]
	public class DirectionsUpdaterHub : Hub
	{
		private readonly DirectionsUpdater _directionsUpdater;

		public DirectionsUpdaterHub() :
			this(DirectionsUpdater.Instance)
		{

		}

		public DirectionsUpdaterHub(DirectionsUpdater directionsUpdater)
		{
			_directionsUpdater = directionsUpdater;
		}

		public IEnumerable<Direction> GetAllDirections()
		{
			return _directionsUpdater.GetDirections();
		}
	}
}