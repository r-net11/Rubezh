using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcClientSdk;
using OpcClientSdk.Da;
using RubezhAPI;
using RubezhAPI.Automation;
using FiresecService.Processor;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<OpcDaServer[]> GetOpcDaServers()
		{
			return new OperationResult<OpcDaServer[]>(OpcDaServersProcessor.GetOpcDaServers());
		}

		public OperationResult<OpcServerStatus> GetOpcDaServerStatus(OpcDaServer server)
		{
			var status = OpcDaServersProcessor.GetServerStatus(server);

			return status != null ? new OperationResult<OpcServerStatus>(status) :
				new OperationResult<OpcServerStatus>
				{
					Errors = new List<string> { string.Format("OPC DA сервер {0} не подключен", server.ServerName) },
					Result = null
				};
		}

		public OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(OpcDaServer server)
		{
			OperationResult<OpcDaElement[]> result;

			var tags = OpcDaServersProcessor.GetOpcDaServerGroupAndTags(server);

			if (tags != null)
			{
				result = new OperationResult<OpcDaElement[]>(tags);
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

		//public OperationResult<TsCDaItemValueResult[]> ReadOpcDaServerTags(OpcDaServer server)
		//{
		//	OperationResult<TsCDaItemValueResult[]> result;

		//	var srv = null; //FindOpcDaServer(server);

		//	if (srv != null)
		//	{
		//		if (srv.IsConnected)
		//		{
		//			var tags = server.Tags.Select(x => new TsCDaItem(new OpcItem(x.ElementName))).ToArray();
		//			var readingResult = srv.Read(tags);
		//			result = new OperationResult<TsCDaItemValueResult[]>(readingResult);
		//		}
		//		else
		//		{
		//			result = new OperationResult<TsCDaItemValueResult[]>
		//			{
		//				Errors = new List<string> { string.Format("OPC DA сервер {0} не подключен", server.ServerName) },
		//				Result = null
		//			};
		//		}
		//	}
		//	else
		//	{
		//		result = new OperationResult<TsCDaItemValueResult[]>
		//		{
		//			Errors = new List<string> 
		//			{ 
		//				string.Format("OPC DA сервер {0} не найден", server.ServerName) 
		//			},
		//			Result = null
		//		};
		//	}
		//	return result;
		//}

		//public OperationResult WriteOpcDaServerTags(OpcDaServer server, TsCDaItemValue[] tagValues)
		//{
		//	OperationResult result;

		//	var srv = null; //FindOpcDaServer(server);

		//	if (srv != null)
		//	{
		//		if (srv.IsConnected)
		//		{
		//			srv.Write(tagValues);
		//			result = new OperationResult();
		//		}
		//		else
		//		{
		//			result = new OperationResult(string.Format(
		//				"OPC DA сервер {0} не подключен", server.ServerName));
		//		}
		//	}
		//	else
		//	{
		//		result = new OperationResult(string.Format(
		//			"OPC DA сервер {0} не найден", server.ServerName));
		//	}
		//	return result;
		//}

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