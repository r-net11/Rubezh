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
				ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.ToArray();

			foreach (var server in OpcDaServers)
			{
				var url = new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, server.Url);
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

		public static OpcDaServer[] GetOpcDaServers()
		{
			return (OpcDiscovery.GetServers(OpcSpecification.OPC_DA_20)
				.Select(srv => new OpcDaServer
				{
					IsChecked = false,
					Login = string.Empty,
					Password = string.Empty,
					ServerName = srv.ServerName,
					Url = srv.Url.ToString()
				})
				.ToArray());
		}

		public static OpcServerStatus GetServerStatus(OpcDaServer server)
		{
			var opcServer = _opcServers.FirstOrDefault(x => x.ServerName == server.ServerName);

			if (opcServer == null)
			{
				return null;
			}
			else
			{
				return opcServer.IsConnected ? opcServer.GetServerStatus() : null;
			}
		}

		static TsCDaBrowseElement[] Browse(TsCDaServer server)
		{
			TsCDaBrowseFilters filters;
			List<TsCDaBrowseElement> elementList;
			TsCDaBrowseElement[] elements;
			TsCDaBrowsePosition position;
			OpcItem path = new OpcItem();

			filters = new TsCDaBrowseFilters();
			filters.BrowseFilter = TsCDaBrowseFilter.All;
			filters.ReturnAllProperties = true;
			filters.ReturnPropertyValues = true;

			elementList = new List<TsCDaBrowseElement>();

			elements = server.Browse(path, filters, out position);

			foreach (var item in elements)
			{
				item.ItemPath = OpcDaTag.ROOT + OpcDaTag.SPLITTER + item.ItemName;
				elementList.Add(item);

				if (!item.IsItem)
				{
					path = new OpcItem(item.ItemPath, item.Name);
					BrowseChildren(path, filters, elementList, server);
				}

			}
			return elementList.ToArray();
		}

		static void BrowseChildren(OpcItem opcItem, TsCDaBrowseFilters filters,
			IList<TsCDaBrowseElement> elementList, TsCDaServer server)
		{
			TsCDaBrowsePosition position;
			OpcItem path;

			var elements = server.Browse(opcItem, filters, out position);

			if (elements != null)
			{
				foreach (var item in elements)
				{
					item.ItemPath = opcItem.ItemPath + OpcDaTag.SPLITTER + item.ItemName;
					elementList.Add(item);

					if (!item.IsItem)
					{
						path = new OpcItem(item.ItemPath, item.ItemName);
						BrowseChildren(path, filters, elementList, server);
					}
				}
			}
		}

		public static OpcDaElement[] GetOpcDaServerGroupAndTags(OpcDaServer server)
		{
			OpcDaElement[] result;
			var opcServer = _opcServers.FirstOrDefault(x => x.ServerName == server.ServerName);

			if (opcServer == null)
			{
				opcServer = (TsCDaServer)OpcDiscovery.GetServers(OpcSpecification.OPC_DA_20)
					.FirstOrDefault(y => y.ServerName == server.ServerName);

				if (opcServer == null)
				{
					result = null;
				}
				else
				{
					opcServer.Connect(new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, server.Url), null);
					result = Browse(opcServer).Select(tag =>
					{
						return tag.IsItem ? OpcDaElement.Create(tag) : OpcDaElement.Create(tag);
					})
					.ToArray(); ;
					opcServer.Disconnect();
				}
			}
			else
			{
				if (opcServer.IsConnected)
				{
					result = Browse(opcServer)
						.Select(tag =>
						{
							return tag.IsItem ? OpcDaElement.Create(tag) : OpcDaElement.Create(tag);
						})
						.ToArray();
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static bool WriteTag(Guid tagId, object value, out string error)
		{
			var opcTag = _tags.FirstOrDefault(t => t.Uid == tagId);
			var server = _opcServers.FirstOrDefault(srv => srv.ServerName == opcTag.ServerName);

			if (value.GetType().ToString() != opcTag.TypeNameOfValue)
			{
				error = "Тип данный заначения тега не соответствует заданному";
				return false;
			}

			var subscription = _subscriptions.FirstOrDefault(s => s.Server == server);
			var tag = subscription.Items.FirstOrDefault(t => t.ItemName == opcTag.ElementName);
			
			TsCDaItemValue item = new TsCDaItemValue(tag);
			item.Value = value;

			var result = server.Write(new TsCDaItemValue[] { item });

			if (result.Length == 1)
			{
				error = string.Format("Code: {0}; Description: {1}; Name: {2}", 
					result[0].Result.Code, result[0].Result.Description(), result[0].Result.Name);
				return !result[0].Result.IsError(); 
			}
			else
			{
				error = string.Format("Неопределённый результат операции записи тега");
				return false;
			}
		}

		public static OpcDaTagValue[] ReadTags(OpcDaServer server)
		{
			return _tags.Where(tag => tag.ServerId == server.Uid).ToArray();
		}

		#endregion

		#region Event handlers

		static void EventHandler_Subscription_DataChangedEvent(object subscriptionHandle, 
			object requestHandle, TsCDaItemValueResult[] values)
		{
			var subscr = _subscriptions.FirstOrDefault(sbs => sbs.ClientHandle == subscriptionHandle);
			var server = _opcServers.FirstOrDefault(srv => srv.ServerName == subscr.Server.ServerName).ServerName;

			if (values.Length > 0)
			{

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