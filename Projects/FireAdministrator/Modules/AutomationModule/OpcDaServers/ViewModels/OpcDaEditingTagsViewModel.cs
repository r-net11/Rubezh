using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using Infrastructure.Common.TreeList;
using AutomationModule.Models;

namespace AutomationModule.ViewModels
{
	public class OpcDaEditingTagsViewModel : SaveCancelDialogViewModel
	{
		public OpcDaEditingTagsViewModel(OpcDaServersViewModel server)
		{
			Title = "Выбрать теги";

			var allTags = OpcDaServerHelper.GetAllTagsFromOpcServer(
				OpcDaServer.OpcDaServer.GetRegistredServers().First(x => x.Id == server.SelectedOpcDaServer.Id))
				.Select(tag => new OpcDaEditingTagsTagViewModel(tag)).ToArray();

			// Получаем список уже выбранных тегов
			// и устанавливаем им признак
			foreach (var x in allTags)
			{
				foreach (var y in server.SelectedOpcDaServer.Tags)
				{
					if (x.Tag.TagId == y.TagId)
					{
						x.IsChecked = true;
					}
				}
			}
			_tags = allTags;
			RootItem = BuildTagsTreeByPath(allTags);
		}
		
		OpcDaEditingTagsTagViewModel[] _tags;

		OpcDaEditingTagsTreeElementViewModel _rootItem;
		public OpcDaEditingTagsTreeElementViewModel RootItem
		{
			get { return _rootItem; }
			private set 
			{
				_rootItem = value;
				OnPropertyChanged(() => RootItem);
				OnPropertyChanged(() => RootItems);
			}
		}

		public OpcDaEditingTagsTreeElementViewModel[] RootItems
		{
			get { return _rootItem.Children.ToArray(); }
		}

		OpcDaEditingTagsTreeElementViewModel _selectedItem;
		public OpcDaEditingTagsTreeElementViewModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged(() => SelectedItem);
			}
		}

		public OpcDaEditingTagsTagViewModel[] SelectedItems { get; private set; }

		protected override bool Save()
		{
			SelectedItems = _tags.Where(x => x.IsChecked).ToArray();
			return base.Save();
		}

		public static OpcDaEditingTagsTreeElementViewModel BuildTagsTreeByPath(OpcDaEditingTagsTagViewModel[] tags)
		{
			// Добавляем корневой элемент
			OpcDaEditingTagsTreeElementViewModel tree = null;
			TreeNodeViewModel currentNode = null;

			// Получаем все теги

			foreach (var item in tags)
			{
				var segments = OpcDaTag.GetGroupNamesAndCheckFormatFromPath(item.Tag.Path);

				for (int i = 0; i < segments.Length; i++)
				{
					if (i == 0)
					{
						if (tree == null)
						{
							tree = new OpcDaEditingTagsGroupViewModel(OpcDaServer.OpcDaTag.RootDirectory);
						}
						currentNode = tree;
					}
					else if (i == (segments.Length - 1))
					{
						// Последний элемент пути всегда тег
						currentNode.Nodes.Add(item);
					}
					else
					{
						// Данный элемент всегда группа тегов
						var group = currentNode.Nodes.Select(g => (OpcDaEditingTagsTreeElementViewModel)g)
							.FirstOrDefault(g => g.ElementName == segments[i] && g.IsTag == false);

						if (group == null)
						{
							var newGroup = new OpcDaEditingTagsGroupViewModel(segments[i]);
							currentNode.Nodes.Add(newGroup);
							currentNode = newGroup;
						}
						else
						{
							currentNode = group;
						}
					}
				}
			}
			return (OpcDaEditingTagsTreeElementViewModel)tree;
		}
	}
}