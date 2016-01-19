using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;
using RubezhAPI;
using GKWebService.Models;
using Microsoft.AspNet.SignalR;

namespace GKWebService.DataProviders
{
	public class DirectionsUpdater
	{
		// Singleton instance
		private readonly static Lazy<DirectionsUpdater> _instance = new Lazy<DirectionsUpdater>(
			() => new DirectionsUpdater(GlobalHost.ConnectionManager.GetHubContext<DirectionsUpdaterHub>().Clients));

		private readonly ConcurrentDictionary<Guid, Direction> _directions = new ConcurrentDictionary<Guid, Direction>();
		private readonly object _updateDirectionLock = new object();
		private volatile bool _updatingDirection;
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
		private Timer _timer;
		Random randomState = new Random(19);

		private DirectionsUpdater(IHubConnectionContext<dynamic> clients)
		{
			Clients = clients;
			LoadDirections();
			_timer = new Timer(UpdateDirections, null, _updateInterval, _updateInterval);
		}

		public static DirectionsUpdater Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		private IHubConnectionContext<dynamic> Clients { get; set; }

		public IEnumerable<Direction> GetDirections()
		{
			return _directions.Values;
		}

		private void LoadDirections()
		{
			_directions.Clear();

			foreach(var realDirection in GKManager.Directions)
			{
				var direction = new Direction
				{
					UID = realDirection.UID,
					No = realDirection.No,
					Name = realDirection.Name,
					State = realDirection.State.StateClass.ToDescription(),
					StateIcon = realDirection.State.StateClass.ToString()
				};
				_directions.TryAdd(direction.UID, direction);
			}
		}

		private void UpdateDirections(object state)
		{
			lock (_updateDirectionLock)
			{
				if (!_updatingDirection)
				{
					_updatingDirection = true;

					foreach (var direction in _directions.Values)
					{
						if (TryUpdateDirection(direction))
						{
							BroadcastDirection(direction);
						}
					}

					_updatingDirection = false;
				}
			}
		}

		private bool TryUpdateDirection(Direction direction)
		{
			direction.State = ((XStateClass)randomState.Next(19)).ToDescription();
			return true;
		}

		private void BroadcastDirection(Direction direction)
		{
			Clients.All.updateDirection(direction);
		}
	}
}