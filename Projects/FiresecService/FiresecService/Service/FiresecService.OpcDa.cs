using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcClientSdk;
using OpcClientSdk.Da;
using RubezhAPI;
using RubezhAPI.Automation;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
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
					result = new OperationResult<OpcServerStatus>(item.GetServerStatus());
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
							return tag.IsItem ? OpcDaElement.Create(tag) : OpcDaElement.Create(tag);
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
	}
}