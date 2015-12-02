using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcDaServerViewModel: BaseViewModel
	{
		#region Constructors

		public OpcDaServerViewModel(RubezhAPI.Automation.OpcDaServer server)
		{
			Server = server;
			ServerName = server.ServerName;
			Id = server.Id;

			#region Для отладки
			//Server.Tags = GetAllTagsFromOpcServer();
			#endregion
		}

		#endregion

		#region Fields And Properties

		public RubezhAPI.Automation.OpcDaServer Server { get; protected set; }
		public string ServerName { get; protected set; }
		public Guid Id { get; protected set; }

		public RubezhAPI.Automation.OpcDaServer ConvertTo()
		{
			return new RubezhAPI.Automation.OpcDaServer 
						{ 
							ServerName = ServerName,
							Id = Id
						};
		}

		public RubezhAPI.Automation.OpcDaTag[] GetAllTagsFromOpcServer()
		{
			var server = OpcDaServer.OpcDaServer.GetRegistredServers().First(s => s.Id == this.Id);

			List<RubezhAPI.Automation.OpcDaTag> tags = new List<RubezhAPI.Automation.OpcDaTag>();

			GetTags(ref tags, server.Tags);

			return tags.ToArray();
		}

		void GetTags(ref List<RubezhAPI.Automation.OpcDaTag> tags, OpcDaServer.OpcDaDirectory directory)
		{
			foreach (var item in directory.Tags)
			{
				if (item.IsDirectory)
				{
					var dir = (OpcDaServer.OpcDaDirectory)item;
					GetTags(ref tags, dir);
				}
				else
				{
					var tag = (OpcDaServer.OpcDaTag)item;
					tags.Add(new RubezhAPI.Automation.OpcDaTag 
					{ 
						Path = tag.FullPath, 
						TagId = tag.TagId, 
						TagName = tag.Name, 
					});
				}
			}
		}

		#endregion
	}
}