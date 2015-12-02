using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.Models
{
	public class OpcDaServerHelper
	{
		#region Methods

		public static RubezhAPI.Automation.OpcDaTag[] GetAllTagsFromOpcServer(OpcDaServer.OpcDaServer server)
		{
			var srv = OpcDaServer.OpcDaServer.GetRegistredServers().First(s => s.Id == server.Id);
			List<RubezhAPI.Automation.OpcDaTag> tags = new List<RubezhAPI.Automation.OpcDaTag>();
			GetTags(ref tags, srv.Tags);
			return tags.ToArray();
		}

		static void GetTags(ref List<RubezhAPI.Automation.OpcDaTag> tags, OpcDaServer.OpcDaDirectory directory)
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