using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using Infrastructure;
using RubezhClient;
using AutomationModule.Models;

namespace AutomationModule.ViewModels
{
	public class OpcDaServersViewModel : MenuViewPartViewModel
	{
		#region Constructors

		public OpcDaServersViewModel()
		{
			Menu = new OpcDaMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			OpcDaServers = new ObservableCollection<OpcDaServerModel>();

			// Загрузка сохранённого списка серверов
			foreach (var opcServer in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaServers)
			{
				var opcServerViewModel = new OpcDaServerModel(opcServer);
				OpcDaServers.Add(opcServerViewModel);
			}
		}

		#endregion

		#region Fields And Properties

		ObservableCollection<OpcDaServerModel> _opcDaServers;

		public ObservableCollection<OpcDaServerModel> OpcDaServers
		{
			get { return _opcDaServers; }
			set 
			{
				_opcDaServers = value;
				OnPropertyChanged(() => OpcDaServers);
			}
		}

		OpcDaServerModel _selectedOpcDaServer;

		public OpcDaServerModel SelectedOpcDaServer
		{
			get { return _selectedOpcDaServer; }
			set
			{
				_selectedOpcDaServer = value;
				Tags = _selectedOpcDaServer == null ? null : _selectedOpcDaServer.Server.Tags;
				OnPropertyChanged(() => SelectedOpcDaServer);
			}
		}

		public RubezhAPI.Automation.OpcDaTag[] Tags 
		{
			get { return SelectedOpcDaServer == null ? null : SelectedOpcDaServer.Server.Tags; }
			set 
			{
				if (SelectedOpcDaServer != null)
				{
					SelectedOpcDaServer.Server.Tags = value;
					OnPropertyChanged(() => Tags);
				}
			}
		}

		#endregion

		#region Methods

		public void Initialize()
		{ 
		}

		#endregion

		#region Commands

		public RelayCommand AddCommand { get; private set; }

		void OnAdd()
		{
			List<OpcDaServerModel> notselectedServers;

			//Cоздаём и передаём список отсутствующих в конфигурационном списке серверов
			var list = OpcDaServer.OpcDaServer.GetRegistredServers().Select(x => new OpcDaServerModel(
				new RubezhAPI.Automation.OpcDaServer { Id = x.Id, ServerName = x.ServerName })).ToList();

			if (OpcDaServers.Count == 0)
			{
				notselectedServers = list;
			}
			else
			{
				notselectedServers = new List<OpcDaServerModel>();

				foreach (var item in list)
				{
					var serv = OpcDaServers.FirstOrDefault(x => x.Id == item.Id);
					if (serv == null)
					{
						notselectedServers.Add(item);
					}
				}
			}

			var addingDialog = new OpcDaAddingServersViewModel(notselectedServers);

			if (Infrastructure.Common.Windows.DialogService.ShowModalWindow(addingDialog))
			{
				foreach (var server in addingDialog.SelectedServers)
				{
					OpcDaServers.Add(server);
					ClientManager.SystemConfiguration.AutomationConfiguration
						.OpcDaServers.Add(server.ConvertTo());
					ServiceFactory.SaveService.AutomationChanged = true;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			ClientManager.SystemConfiguration.AutomationConfiguration
				.OpcDaServers.Remove(SelectedOpcDaServer.Server);
			ServiceFactory.SaveService.AutomationChanged = true;

			OpcDaServers.Remove(SelectedOpcDaServer);
			SelectedOpcDaServer = OpcDaServers.FirstOrDefault();
		}

		bool CanDelete()
		{
			return SelectedOpcDaServer != null;
		}

		/// <summary>
		/// Редактируем список тегов OPC сервера
		/// </summary>
		public RelayCommand EditCommand { get; private set; }

		void OnEdit()
		{
			var allTags = SelectedOpcDaServer.GetAllTagsFromOpcServer()
				.Select(tag => new OpcDaEditingTagsTagViewModel(tag)).ToArray();

			// Получаем список уже выбранных тегов
			// и устанавливаем им признак
			foreach (var x in allTags)
			{ 
				foreach(var y in SelectedOpcDaServer.Server.Tags)
				{
					if (x.Tag.TagId == y.TagId)
					{
						x.IsChecked = true;
					}
				}
			}

			var addingDialog = new OpcDaEditingTagsViewModel(allTags);

			if (Infrastructure.Common.Windows.DialogService.ShowModalWindow(addingDialog))
			{
				Tags = (addingDialog.SelectedItems
					.Select(x => x.Tag)).ToArray();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEdit()
		{
			return SelectedOpcDaServer != null;
		}

		#endregion
	}
}