using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class OpcDaEditingTagsViewModel : SaveCancelDialogViewModel
	{
		public OpcDaEditingTagsViewModel(OpcDaEditingTagsTagViewModel[] tags)
		{
			Title = "Выбрать теги";

			if (tags == null)
			{
				throw new ArgumentNullException("tagsTree");
			}
			_tags = tags;
			RootItem = BuildTagsTreeByPath(tags);
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
			var list = new List<OpcDaEditingTagsTagViewModel>();

			foreach (var item in _tags)
			{
				if (item.IsChecked)
				{
					list.Add(item);
				}
			}
			SelectedItems = list.ToArray();
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