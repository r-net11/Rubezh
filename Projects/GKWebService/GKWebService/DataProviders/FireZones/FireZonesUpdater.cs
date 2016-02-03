using System;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhClient;

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
            lock (_startStatesMonitoringLock)
            {
                //Получаем состояния зон
                _currentStates = ClientManager.FiresecService.GKGetStates().ZoneStates;

                _timer = new Timer(_refreshZoneState, null, _updateInterval, _updateInterval);
            }
        }

        /// <summary>
        /// Метод, обновляющий статус зоны
        /// </summary>
        private void _refreshZoneState(object parameter)
        {
            //Получаем новые данные о состоянии зон
            _newStates = ClientManager.FiresecService.GKGetStates().ZoneStates;

            //Если есть изменения, то отправляем данные на клиент и биндим в _currentStates
            for (int i = 0; i < _newStates.Count; i++)
            {
                if (_newStates[i].StateClass != _currentStates[i].StateClass && _newStates[i].UID == _currentStates[i].UID)
                {
                    _currentStates[i] = _newStates[i];
                    Clients.All.RefreshZoneState(FireZonesDataProvider.Instance.GetFireZones()[i]);
                }
            }
        }

        /// <summary>
        /// Новые состояния зон
        /// </summary>
        private List<GKState> _newStates;

        /// <summary>
        /// Текущие состояния зон
        /// </summary>
        private List<GKState> _currentStates;
    }
}
