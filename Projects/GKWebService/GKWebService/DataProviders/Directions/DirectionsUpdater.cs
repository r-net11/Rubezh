using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Controls.Converters;
using GKModule.Converters;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;
using RubezhAPI;
using GKWebService.Models;
using GKWebService.Models.GK;
using GKWebService.Utils;
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
					StateIcon = realDirection.State.StateClass.ToString(),
					OnDelay = realDirection.State.OnDelay,
					HoldDelay = realDirection.State.HoldDelay,
					GKDescriptorNo = realDirection.GKDescriptorNo,
					Delay = realDirection.Delay,
					Hold = realDirection.Hold,
					DelayRegimeName = realDirection.DelayRegime.ToDescription(),
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
			var stateClass = (XStateClass)randomState.Next(19);
			var stateClasses = new List<XStateClass> { stateClass };
			direction.State = stateClass.ToDescription();
			direction.StateIcon = stateClass.ToString();
			direction.StateColor = "'#" + new XStateClassToColorConverter2().Convert(stateClass, null, null, null).ToString().Substring(3) + "'";
			direction.StateClasses = new List<DirectionStateClass>{ new DirectionStateClass(stateClass)};
			direction.HasOnDelay = stateClasses.Contains(XStateClass.TurningOn) && direction.OnDelay > 0;
			direction.HasHoldDelay = stateClasses.Contains(XStateClass.On) && direction.HoldDelay > 0;
			direction.ControlRegime = stateClasses.Contains(XStateClass.Ignore)
				? DeviceControlRegime.Ignore
				: !stateClasses.Contains(XStateClass.AutoOff) ? DeviceControlRegime.Automatic : DeviceControlRegime.Manual;
			direction.ControlRegimeIcon = "data:image/gif;base64," + InternalConverter.GetImageResource(((string) new DeviceControlRegimeToIconConverter().Convert(direction.ControlRegime, null, null, null)) ?? string.Empty).Item1;
			direction.ControlRegimeName = direction.ControlRegime.ToDescription();
			direction.CanSetAutomaticState = (direction.ControlRegime != DeviceControlRegime.Automatic);
			direction.CanSetManualState = (direction.ControlRegime != DeviceControlRegime.Manual);
			direction.CanSetIgnoreState = (direction.ControlRegime != DeviceControlRegime.Ignore);
			direction.IsControlRegime = (direction.ControlRegime == DeviceControlRegime.Manual);
			return true;
		}

		private void BroadcastDirection(Direction direction)
		{
			Clients.All.updateDirection(direction);
		}
	}
}