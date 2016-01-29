using System;
using System.Threading;
using GKWebService.Models.FireZone;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;

namespace GKWebService.DataProviders.FireZones
{
    public class FireZonesUpdater
    {
        // Singleton instance
		private static readonly Lazy<FireZonesUpdater> _instance = new Lazy<FireZonesUpdater>(
            () => new FireZonesUpdater(GlobalHost.ConnectionManager.GetHubContext<FireZonesUpdaterHub>().Clients));

		private readonly object _startStatesMonitoringLock = new object();
		private readonly object _testSendMessageLock = new object();
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(100);//old value 250
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

        public void StartStatesMonitoring()
        {
			lock (_startStatesMonitoringLock) {
                _data = FireZonesDataProvider.Instance.GetFireZones();
                
                _currentState = new List<String>();
                //назначаем текущий статус зоны
			    foreach (var item in _data)
			    {
                    _currentState.Add(item.StateLabel);
			    }

                _timer = new Timer(_refreshZoneState, null, _updateInterval, _updateInterval);
			}
		}

        /// <summary>
        /// Метод, обновляющий статус зоны
        /// </summary>
        private void _refreshZoneState(object parameter)
        {
            //Получаем текущие данные
            _data = FireZonesDataProvider.Instance.GetFireZones();
            bool isNewStates = false;
            foreach (var item in _data)
            {
                if (item.StateLabel != _currentState[_data.IndexOf(item)])
                {
                    _currentState[_data.IndexOf(item)] = item.StateLabel;
                    isNewStates = true;
                }    
            }

            if (isNewStates)
            {
                Clients.All.RefreshZoneState(_data);
                isNewStates = false;
            }
            
        }

        /// <summary>
        /// Данные о зоне
        /// </summary>
        private List<FireZone> _data;

        /// <summary>
        /// Текущий статус зоны (ConnectionLost, Norm, etc...)
        /// </summary>
        private List<String> _currentState;
    }
}
