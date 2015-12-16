using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.Models
{
	public class OpcDaServerHelper
	{
		#region Methods

		public static RubezhAPI.Automation.OpcDaTag[] GetAllTagsFromOpcServer(OpcFoundation.OpcDaServer server)
		{
			var srv = OpcFoundation.OpcDaServer.GetRegistredServers().First(s => s.Id == server.Id);
			List<RubezhAPI.Automation.OpcDaTag> tags = new List<RubezhAPI.Automation.OpcDaTag>();
			GetTags(ref tags, srv.Tags);
			return tags.ToArray();
		}

		static void GetTags(ref List<RubezhAPI.Automation.OpcDaTag> tags, OpcFoundation.OpcDaDirectory directory)
		{
			foreach (var item in directory.Items)
			{
				if (item.IsDirectory)
				{
					var dir = (OpcFoundation.OpcDaDirectory)item;
					GetTags(ref tags, dir);
				}
				else
				{
					var tag = (OpcFoundation.OpcDaTag)item;
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