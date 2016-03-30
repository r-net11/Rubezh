using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcClientSdk.Da;
using OpcClientSdk;
using RubezhAPI.Automation;
using RubezhClient;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientEditingTagsViewModel : SaveCancelDialogViewModel
	{
		#region Constructors

		public OpcDaClientEditingTagsViewModel(OpcDaClientViewModel vm)
		{
			Title = "Структура групп тегов";

			OpcDaClientViewModel = vm;
			
			var result = ClientManager.FiresecService
				.GetOpcDaServerGroupAndTags(FiresecServiceFactory.UID, OpcDaClientViewModel.SelectedOpcServer);

			if (result.HasError)
			{
				return;
			}
			else
			{
				// Строим дерево для отображения
				Elements = result.Result;
				RootElement = OpcDaClientElementViewModel.Create(result.Result);
			}

			//var resultDisconnect = ClientManager.FiresecService
			//	.DisconnectFromOpcDaServer(OpcDaClientViewModel.SelectedOpcServer);

			var children = RootElement.GetAllChildren(false).Where(x => x.Element.IsTag);

			// Отмечаем уже выбранные теги
			foreach (var item in OpcDaClientViewModel.SelectedTags)
			{
				var element = children.FirstOrDefault(x =>
					x.Element.ElementName == item.ElementName && x.Element.Path == item.Path);
				
				if (element != null)
				{
					var node = (OpcDaTag)element.Element;
					node.IsChecked = true;
					node.Uid = item.Uid;
				}
			}
		}

		#endregion

		#region Fields And Properties

		OpcDaClientViewModel OpcDaClientViewModel { get; set; }
		TsCDaServer OpcServer { get; set; }


		OpcDaClientElementViewModel RootElement { get; set; }
		OpcDaElement[] Elements { get; set; }

		public OpcDaClientElementViewModel[] RootItems
		{
			get 
			{ 
				return RootElement.Children.ToArray(); 
			}
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
			var checkedItems = Elements
				.Where(x => x.IsTag)
				.Select(x => (OpcDaTag)x)
				.Where(tag => tag.IsChecked);

			var newItems = checkedItems.Where(x => x.Uid == Guid.Empty);

			foreach (var item in newItems)
			{
				item.Uid = Guid.NewGuid();
			}

			OpcDaClientViewModel.SelectedTags = checkedItems.ToArray();
			return base.Save();
		}

		#endregion
	}
}