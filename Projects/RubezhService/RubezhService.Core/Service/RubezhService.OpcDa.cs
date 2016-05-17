using RubezhService.Processor;
using OpcClientSdk;
using OpcClientSdk.Da;
using RubezhAPI;
using RubezhAPI.Automation;
using System;
using System.Collections.Generic;

namespace RubezhService.Service
{
	public partial class RubezhService
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

		public OperationResult<OpcDaTagValue[]> ReadOpcDaServerTags(OpcDaServer server)
		{
			var values = OpcDaServersProcessor.ReadTags(server);
			return new OperationResult<OpcDaTagValue[]>(values);
		}

		public OperationResult<bool> WriteOpcDaTag(Guid tagId, object value)
		{
			OperationResult<bool> result;
			string errorDescription;
			if (OpcDaServersProcessor.WriteTag(tagId, value, out errorDescription))
			{
				result = new OperationResult<bool>(true);
			}
			else
			{
				result = new OperationResult<bool>(false);
				result.Errors.Add(errorDescription);
			}

			return result;
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