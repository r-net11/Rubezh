using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcClientSdk.Da;
using OpcClientSdk;
using RubezhAPI.Automation;
using AutomationModule.Models;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientEditingTagsViewModel : SaveCancelDialogViewModel, IDisposable
	{
		#region Constructors

		public OpcDaClientEditingTagsViewModel(OpcDaClientViewModel vm)
		{
			Title = "Структура групп тегов";

			OpcDaClientViewModel = vm;

			// Подключаемся к OPC Серверу
			OpcServer = new TsCDaServer();
			var url = new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA,
				OpcDaClientViewModel.SelectedOpcServer.Url);
			OpcServer.Connect(url, null);	// только для локального серера;
											// во второй параметр передаются данные для 
											// авторизации пользователя на удалённом сервере
			// Получаем структуру тегов и групп данного сервера
			var elements = Browse();
			TagsAndGroups = new TsOpcTagsStructure(elements);

			// Строим дерево для отображения
			BuildTreeListElemnts();

			// Отмечаем уже выбранные теги
			foreach (var item in OpcDaClientViewModel.SelectedTags)
			{
				//var list = RootElement.GetAllChildren();
				var list = new List<OpcDaClientElementViewModel>();
				GetAllChildren(list);
				
				var element = list.FirstOrDefault(x =>
					x.Element.Element.ItemName == item.TagName && x.Element.Element.ItemPath == item.Path);
				
				if (element != null)
				{
					element.Element.IsChecked = true;
				}
			}
		}
		
		#endregion

		#region Fields And Properties

		OpcDaClientViewModel OpcDaClientViewModel { get; set; }
		TsCDaServer OpcServer { get; set; }
		OpcDaClientElementViewModel RootElement { get; set; }
		TsOpcTagsStructure TagsAndGroups { get; set; }

		public OpcDaClientElementViewModel[] RootItems
		{
			get { return RootElement.Children.ToArray(); }
		}

		OpcDaClientElementViewModel _selectedItem;
		public OpcDaClientElementViewModel SelectedItem
		{
			get { return _selectedItem; }
			set 
			{
				_selectedItem = value;
				OnPropertyChanged(() => SelectedItem);
			}
		}

		#endregion

		#region Methods

		protected override bool Save()
		{
			return base.Save();
		}

		public void Dispose()
		{
			if (OpcServer != null)
			{
				if (OpcServer.IsConnected)
				{
					OpcServer.Disconnect();
				}
				OpcServer.Dispose();
			}
		}

		public override void OnClosed()
		{
			var result = new List<OpcDaTag>();

			var checkedItems = TagsAndGroups.AllElements
				.Where(x => x.IsChecked)
				.Select(y => new OpcDaTag { TagName = y.Element.ItemName, Path = y.Element.ItemPath })
				.ToArray();

			foreach (var item in checkedItems)
			{
				var tag = OpcDaClientViewModel.SelectedTags
					.FirstOrDefault(x => x.TagName == item.TagName && x.Path == item.Path);

				if (tag == null)
				{
					item.Uid = Guid.NewGuid();
					result.Add(item);
				}
				else
				{
					result.Add(tag);
				}

			}

			OpcDaClientViewModel.SelectedTags = result.ToArray();

			Dispose();
			base.OnClosed();
		}

		TsCDaBrowseElement[] Browse()
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

			elements = OpcServer.Browse(path, filters, out position);

			foreach (var item in elements)
			{
				item.ItemPath = OpcDaTag.ROOT + OpcDaTag.SPLITTER + item.ItemName;
				elementList.Add(item);

				if (!item.IsItem)
				{
					path = new OpcItem(item.ItemPath, item.Name);
					BrowseChildren(path, filters, elementList);
				}

			}
			return elementList.ToArray();
		}

		void BrowseChildren(OpcItem opcItem, TsCDaBrowseFilters filters,
			IList<TsCDaBrowseElement> elementList)
		{
			TsCDaBrowsePosition position;
			OpcItem path;

			var elements = OpcServer.Browse(opcItem, filters, out position);

			if (elements != null)
			{
				foreach (var item in elements)
				{
					item.ItemPath = opcItem.ItemPath + OpcDaTag.SPLITTER + item.ItemName;
					elementList.Add(item);

					if (!item.IsItem)
					{
						path = new OpcItem(item.ItemPath, item.ItemName);
						BrowseChildren(path, filters, elementList);
					}
				}
			}
		}

		void BuildTreeListElemnts()
		{
			RootElement = new OpcDaClientElementViewModel(); // Создаём корневую группу
			foreach (var item in TagsAndGroups.Items)
			{
				GetChildren(RootElement, item);
			}
		}

		void GetChildren(OpcDaClientElementViewModel parent, TsOpcElement elementParent)
		{
			var element = new OpcDaClientElementViewModel(elementParent);
			parent.Nodes.Add(element);
			
			if (!elementParent.Element.IsItem)
			{
				foreach (var item in elementParent.Items)
				{
					GetChildren(element, item);
				}
			}
		}

		void GetAllChildren(List<OpcDaClientElementViewModel> list, OpcDaClientElementViewModel child = null)
		{
			if (child == null)
			{
				child = RootElement;
			}

			foreach (var node in child.Nodes)
			{
				var element = (OpcDaClientElementViewModel)node;
				if (element.IsTag)
				{
					list.Add(element);
				}
				else
				{
					GetAllChildren(list, element);
				}
			}
		}

		#endregion
	}
}