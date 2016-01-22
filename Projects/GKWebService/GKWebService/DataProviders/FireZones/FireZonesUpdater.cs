using System;
using System.Threading;
using GKWebService.Models.FireZone;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKWebService.DataProviders.FireZones
{
    public class FireZonesUpdater
    {
        // Singleton instance
		private static readonly Lazy<FireZonesUpdater> _instance = new Lazy<FireZonesUpdater>(
            () => new FireZonesUpdater(GlobalHost.ConnectionManager.GetHubContext<FireZonesUpdaterHub>().Clients));

		private readonly object _testBroadcastLock = new object();
		private readonly object _testSendMessageLock = new object();
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
		private volatile bool _sendingTestMessage;
		private Timer _timer;

        private FireZonesUpdater(IHubConnectionContext<dynamic> clients)
        {
			Clients = clients;
		}

		private IHubConnectionContext<dynamic> Clients { get; set; }

        /// <summary>
        /// Инстанс этого класса
        /// </summary>
        public static FireZonesUpdater Instance { get { return _instance.Value; } }

		public void StartTestBroadcast() {
			lock (_testBroadcastLock) {
                _data = FireZonesDataProvider.Instance.GetZone();
                
                //назначаем текущий статус зоны
			    _currentState = _data.StateLabel;

                _timer = new Timer(_refreshZoneState, null, _updateInterval, _updateInterval);
			}
		}

        /// <summary>
        /// Метод, обновляющий статус зоны
        /// </summary>
        private void _refreshZoneState(object parameter)
        {
            //Получаем текущие данные
            _data = FireZonesDataProvider.Instance.GetZone();

            if (_data.StateLabel != _currentState)
            {
                _currentState = _data.StateLabel;
                Clients.All.RefreshZoneState(_data.StateImageSource.Item1);
            }
        }

        /// <summary>
        /// Данные о зоне
        /// </summary>
        private FireZone _data { get; set; }

        /// <summary>
        /// Текущий статус зоны (ConnectionLost, Norm, etc...)
        /// </summary>
        private String _currentState { get; set; }
    }
}
