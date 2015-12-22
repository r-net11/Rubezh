using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhDAL.DataClasses;
using RubezhAPI.License;
using OpcClientSdk;
using OpcClientSdk.Da;
using RubezhAPI.Automation;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public partial class FiresecService : IFiresecService
	{
		ClientCredentials CurrentClientCredentials;
		public static ServerState ServerState { get; set; }

		void InitializeClientCredentials(ClientCredentials clientCredentials)
		{
			clientCredentials.ClientIpAddress = "127.0.0.1";
			clientCredentials.ClientIpAddressAndPort = "127.0.0.1:0";
			clientCredentials.UserName = clientCredentials.UserName;
			try
			{
				if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
				{
					var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
					clientCredentials.ClientIpAddress = endpoint.Address;
					clientCredentials.ClientIpAddressAndPort = endpoint.Address + ":" + endpoint.Port.ToString();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.InitializeClientCredentials");
			}
		}

		public OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew)
		{
			if (DbService.ConnectionOperationResult.HasError && clientCredentials.ClientType != ClientType.Administrator)
				return OperationResult<bool>.FromError("Отсутствует подключение к БД");
			clientCredentials.ClientUID = uid;
			InitializeClientCredentials(clientCredentials);

			var operationResult = Authenticate(clientCredentials);
			if (operationResult.HasError)
				return operationResult;

			CurrentClientCredentials = clientCredentials;
			if (ClientsManager.Add(uid, clientCredentials))
				AddJournalMessage(JournalEventNameType.Вход_пользователя_в_систему, null);
			return operationResult;
		}

		public void Disconnect(Guid uid)
		{
			var clientInfo = ClientsManager.GetClientInfo(uid);
			if (clientInfo != null)
			{
				clientInfo.IsDisconnecting = true;
				clientInfo.WaitEvent.Set();
				if (clientInfo.ClientCredentials != null)
				{
					AddJournalMessage(JournalEventNameType.Выход_пользователя_из_системы, null);
				}
			}
			ClientsManager.Remove(uid);
			Logger.Error("Bug catching (RG-362). FiresecService.Disconnect");
		}

		public OperationResult<ServerState> GetServerState()
		{
			return new OperationResult<ServerState>(ServerState);
		}

		public string Test(string arg)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				databaseService.PassJournalTranslator.InsertPassJournalTestData();
			}
			return "Test";
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return ConfigurationCashHelper.GetSecurityConfiguration();
		}

		public string Ping()
		{
			try
			{
				if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
				{
					var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
					return endpoint.Address;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.Ping");
			}
			return null;
		}

		public OperationResult ResetDB()
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ResetDB();
			}
		}
		
		public OperationResult<FiresecLicenseInfo> GetLicenseInfo()
		{
			return new OperationResult<FiresecLicenseInfo>(LicenseManager.CurrentLicenseInfo);
		}

		#region OPC DA Servers
		/// <summary>
		/// Пул OPC сереверов
		/// </summary>
		List<TsCDaServer> _OpcDaServerPool = new List<TsCDaServer>();
		/// <summary>
		/// Активные подписки
		/// </summary>
		List<TsCDaSubscription> _OpcDaSubscriptions = new List<TsCDaSubscription>();

		public OperationResult<OpcDaServer[]> GetOpcDaServers()
		{
			return new OperationResult<OpcDaServer[]>(OpcDiscovery.GetServers(OpcSpecification.OPC_DA_20)
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

		public OperationResult ConnectToOpcDaServer(OpcDaServer server)
		{
			var item = FindOpcDaServer(server);
			
			if (item != null)
			{
				if (!item.IsConnected)
				{
					item.Connect(new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, server.Url), null);
				}
			}
			else
			{
				item = new TsCDaServer();
				item.Connect(new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, server.Url), null);
				_OpcDaServerPool.Add(item);
			}

			return new OperationResult { HasError = false, Error = null, Warnings = null };
		}

		public OperationResult DisconnectFromOpcDaServer(OpcDaServer server)
		{
			var item = FindOpcDaServer(server);
			
			if ((item != null) && (item.IsConnected))
			{
				// TODO: необходимо уничтожить все подписки
				item.Disconnect();
			}
			return new OperationResult { HasError = false, Error = null, Warnings = null, };
		}

		public OperationResult<OpcServerStatus> GetOpcDaServerStatus(OpcDaServer server)
		{
			OperationResult<OpcServerStatus> result;

			var item = FindOpcDaServer(server);
			
			if (item == null)
			{
 				result = new OperationResult<OpcServerStatus>
				{
					Errors = new List<string> { string.Format("OPC DA сервер {0} не найден", server.ServerName) },
					Result = null
				};
			}
			else
			{
				if (item.IsConnected)
				{
					result = new OperationResult<OpcServerStatus>
					{
						Errors = null,
						Result = item.GetServerStatus()
					};
				}
				else
				{
					result = new OperationResult<OpcServerStatus>
					{
						Errors = new List<string> { string.Format("OPC DA сервер {0} не подключен", server.ServerName) },
						Result = null
					};
				}
			}
			return result;
		}

		public OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(OpcDaServer server)
		{
			OperationResult<OpcDaElement[]> result;

			var item = FindOpcDaServer(server);

			if (item != null)
			{
				if (item.IsConnected)
				{
					var tags = Browse(item)
						.Select(tag => 
						{
							return tag.IsItem ? (OpcDaElement)(new OpcDaTag { Path = tag.ItemPath, ElementName = tag.ItemName }) :
								(OpcDaElement)(new OpcDaGroup { Path = tag.ItemPath, ElementName = tag.ItemName });
						})
						.ToArray();
					result = new OperationResult<OpcDaElement[]>(tags);
				}
				else
				{
					result = new OperationResult<OpcDaElement[]>
					{
						Errors = new List<string> { string.Format("OPC DA сервер {0} не подключен", server.ServerName) },
						Result = null
					};
				}
			}
			else
			{
				result = new OperationResult<OpcDaElement[]>
				{
					Errors = new List<string> { string.Format("OPC DA сервер {0} не найден", server.ServerName) },
					Result = null
				};
			}
			return result;
		}

		TsCDaBrowseElement[] Browse(TsCDaServer server)
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

		void BrowseChildren(OpcItem opcItem, TsCDaBrowseFilters filters,
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

		public OperationResult<TsCDaItemValueResult[]> ReadOpcDaServerTags(OpcDaServer server)
		{
			OperationResult<TsCDaItemValueResult[]> result;

			var srv = FindOpcDaServer(server);

			if (srv != null)
			{
				if (srv.IsConnected)
				{
					var tags = server.Tags.Select(x => new TsCDaItem(new OpcItem(x.ElementName))).ToArray();
					var readingResult = srv.Read(tags);
					result = new OperationResult<TsCDaItemValueResult[]>(readingResult);
				}
				else
				{
					result = new OperationResult<TsCDaItemValueResult[]> 
					{
						Errors = new List<string> { string.Format("OPC DA сервер {0} не подключен", server.ServerName) },
						Result = null
					};
				}
			}
			else
			{
				result = new OperationResult<TsCDaItemValueResult[]>
				{
					Errors = new List<string> 
					{ 
						string.Format("OPC DA сервер {0} не найден", server.ServerName) 
					},
					Result = null
				};
			}
			return result;
		}

		public OperationResult WriteOpcDaServerTags(OpcDaServer server, TsCDaItemValue[] tagValues)
		{
			OperationResult result;

			var srv = FindOpcDaServer(server);

			if (srv != null)
			{
				if (srv.IsConnected)
				{
					srv.Write(tagValues);
					result = new OperationResult();
				}
				else
				{
					result = new OperationResult(string.Format(
						"OPC DA сервер {0} не подключен", server.ServerName));
				}
			}
			else
			{
				result = new OperationResult(string.Format(
					"OPC DA сервер {0} не найден", server.ServerName));
			}
			return result;
		}

		TsCDaServer FindOpcDaServer(OpcDaServer server)
		{
			return _OpcDaServerPool.FirstOrDefault(x =>
				x.Url.ToString() == server.Url && x.ServerName == server.ServerName);
		}

		void BrowseChildren(TsCDaServer server, OpcItem opcItem, TsCDaBrowseFilters filters,
			IList<TsCDaBrowseElement> elementList)
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
						BrowseChildren(server, path, filters, elementList);
					}
				}
			}
		}

		#endregion
	}
}