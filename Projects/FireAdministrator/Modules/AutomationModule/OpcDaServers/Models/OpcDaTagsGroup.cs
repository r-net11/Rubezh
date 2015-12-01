using System;
using System.Collections.Generic;
using OpcDaServer;

namespace AutomationModule.OpcDaServers.Models
{
	//public class OpcDaTagsGroup
	//{
	//	public string GroupName { get; private set; }
	//	public OpcDaTagsGroup ParentGroup { get; private set; }

	//	public List<OpcDaTagsGroup> _groups = new List<OpcDaTagsGroup>();
	//	public OpcDaTagsGroup[] Groups { get { return _groups.ToArray(); } }

	//	public List<OpcDaTag> _tags = new List<OpcDaTag>();
	//	public OpcDaTag[] Tags { get { return _tags.ToArray(); } }
	//	public string Path { get; private set; }
	//	public bool IsRootGroup
	//	{
	//		get { return ParentGroup == null; }
	//	}

	//	public static OpcDaTagsGroup GetTags(OpcDaServer.OpcDaServer server)
	//	{
	//		OpcDaDirectory directory;
	//		OpcDaTagsGroup group = new OpcDaTagsGroup();

	//		if ((server.Tags.IsDirectory) && (server.Tags.IsRoot))
	//		{
	//			directory = server.Tags;
	//		}
	//		else
	//		{
	//			throw new Exception("OPC Сервер вернул не корневую директорию");
	//		}

	//		// Корневая группа
	//		group.GroupName = directory.DirectoryName;
	//		BuildTagsTree(ref group, directory);

	//		return group;
	//	}

	//	private static void BuildTagsTree(ref OpcDaTagsGroup group, OpcDaDirectory directory)
	//	{
	//		foreach (var item in directory.Tags)
	//		{
	//			if (item.IsDirectory)
	//			{
	//				var internalDirectory = (OpcDaDirectory)item;
	//				var newGroup = new OpcDaTagsGroup { GroupName = internalDirectory.DirectoryName };
	//				group._groups.Add(newGroup);
	//				BuildTagsTree(ref newGroup, internalDirectory);
	//			}
	//			else
	//			{
	//				var baseTag = (OpcDaServer.OpcDaTag)item;
	//				var tag = new OpcDaTag
	//				{
	//					Path = baseTag.FullPath,
	//					TagId = baseTag.TagId,
	//					TagName = baseTag.Name,
	//					TagsGroups = group
	//				};
	//				group._tags.Add(tag);
	//			}
	//		}
	//	}
	//}
}
