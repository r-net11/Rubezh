using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI.Automation;
using OpcClientSdk.Da;
using OpcClientSdk;

namespace FiresecService.Processor
{
	public static class OpcDaServersProcessor
	{
		static OpcDaServersProcessor()
		{
			_opcServers = new List<TsCDaServer>();
			_subscriptions = new List<TsCDaSubscription>();
			_tags = new List<OpcDaTagValue>();
		}

		#region Fields And Properties

		static List<TsCDaServer> _opcServers;
		static List<TsCDaSubscription> _subscriptions;
		static List<OpcDaTagValue> _tags;

		public static OpcDaServer[] OpcDaServers { get; private set; }

		#endregion

		#region Methods

		public static void Start()
		{
			TsCDaItemValue x = new TsCDaItemValue();
			
			OpcDaServers =
				ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaServers.ToArray();

			foreach (var server in OpcDaServers)
			{
				var url = new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, server.Url.ToString());
				var opcServer = new TsCDaServer();
				opcServer.Connect(url, null);
				opcServer.ServerShutdownEvent += EventHandler_ServerShutdownEvent;
				_opcServers.Add(opcServer);

				// Создаём объект подписки
				var id = Guid.NewGuid().ToString();
				var subscriptionState = new TsCDaSubscriptionState
				{
					Name = id,
					ClientHandle = id,
					Deadband = 0,
					UpdateRate = 1000,
					KeepAlive = 10000
				};

				var subscription = (TsCDaSubscription)opcServer.CreateSubscription(subscriptionState);
				_subscriptions.Add(subscription);

				// Добавляем в объект подписки выбранные теги
				List<TsCDaItem> list = new List<TsCDaItem>();
				
				list.AddRange(server.Tags.Select(tag => 
					new TsCDaItem
					{
						ItemName = tag.ElementName,
						ClientHandle = tag.ElementName // Уникальный Id определяемый пользователем
					}));

				// Добавляем теги и проверяем результат данной операции
				var results = subscription.AddItems(list.ToArray());

				var errors = results.Where(result => result.Result.IsError());

				if (errors.Count() > 0)
				{
					StringBuilder msg = new StringBuilder();
					msg.Append("Не удалось добавить теги для подписки. Возникли ошибки в тегах:");
					foreach (var error in errors)
					{
						msg.Append(String.Format("ItemName={0} ClientHandle={1} Description={2}; ",
							error.ItemName, error.ClientHandle, error.Result.Description()));
					}
					throw new InvalidOperationException(msg.ToString());
				}

				subscription.DataChangedEvent += EventHandler_Subscription_DataChangedEvent;

				_tags.AddRange(server.Tags.Select(tag => new OpcDaTagValue
				{
					ElementName = tag.ElementName,
					TagId = tag.TagId,
					Uid = tag.Uid,
					Path = tag.Path,
					TypeNameOfValue = tag.TypeNameOfValue,
					AccessRights = tag.AccessRights,
					ScanRate = tag.ScanRate,
					ServerId = server.Uid,
					ServerName = server.ServerName
				}));
			}
		}

		public static void Stop()
		{
			_tags.Clear();

			foreach (var server in _opcServers)
			{
				// Прекращем все подписки
				var subscriptions = _subscriptions.Where(x => x.Server == server);
				
				foreach (var subscription in subscriptions)
				{
					server.CancelSubscription(subscription);
					subscription.DataChangedEvent -= EventHandler_Subscription_DataChangedEvent;
					subscription.Dispose();
				}

				_opcServers.Remove(server);

				// Отключаемся от сервера
				server.Disconnect();
				server.ServerShutdownEvent -= EventHandler_ServerShutdownEvent;
				server.Dispose();
			}
		}

		public static OpcServerStatus[] GetServerStatuses()
		{
			List<OpcServerStatus> statuses = new List<OpcServerStatus>();

			foreach (var server in _opcServers)
			{
				statuses.Add(server.GetServerStatus());
			}

			return statuses.ToArray();
		}

		public static void WriteTag(Guid serverId, Guid tagId, object value)
		{
			var serverName = OpcDaServers.FirstOrDefault(x => x.Uid == serverId).ServerName;
			var server = _opcServers.FirstOrDefault(srv => srv.ServerName == serverName);
			
			var opcTag = _tags.FirstOrDefault(tgs => tgs.ServerId == serverId);
			if (value.GetType().ToString() != opcTag.TypeNameOfValue)
			{
				throw new ArgumentException("Тип данный заначения тега не соответствует заданному");
			}

			var subscription = _subscriptions.FirstOrDefault(s => s.Server == server);
			var tag = subscription.Items.FirstOrDefault(t => t.ItemName == opcTag.ElementName);
			
			TsCDaItemValue item = new TsCDaItemValue(tag);
			item.Value = value;

			var result = server.Write(new TsCDaItemValue[] { item });
		}

		#endregion

		#region Event handlers

		static void EventHandler_Subscription_DataChangedEvent(object subscriptionHandle, 
			object requestHandle, TsCDaItemValueResult[] values)
		{
			if (values.Length > 0)
			{
				var subscr = _subscriptions.FirstOrDefault(sbs => sbs.ServerHandle == values[0].ServerHandle);
				var server = _opcServers.FirstOrDefault(srv => srv.ServerName == subscr.Server.ServerName).ServerName;

				foreach (var result in values)
				{
					var tag = _tags.FirstOrDefault(tg => tg.ServerName == server);
					tag.Quality = result.Quality.GetCode();
					tag.Timestamp = result.Timestamp;
					tag.Value = result.Value;
					tag.OperationResult = result.Result.IsSuccess();
				}
			}
		}

		static void EventHandler_ServerShutdownEvent(string reason)
		{
			// Ищем отключившийся сервер
		}

		#endregion
	}
}