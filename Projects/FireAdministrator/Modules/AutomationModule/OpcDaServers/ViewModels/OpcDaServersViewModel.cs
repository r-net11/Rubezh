﻿using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using Infrastructure;
using RubezhClient;
using AutomationModule.Models;
using Infrastructure.Common.Windows;

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
			ConnectCommand = new RelayCommand(OnConnectToServer, CanConnectToServer);

			OpcDaServers = new ObservableCollection<RubezhAPI.Automation.OpcDaServer>();

			// Загрузка сохранённого списка серверов
			foreach (var opcServer in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaServers)
			{
				OpcDaServers.Add(opcServer);
			}
		}

		#endregion

		#region Fields And Properties

		ObservableCollection<RubezhAPI.Automation.OpcDaServer> _opcDaServers;

		public ObservableCollection<RubezhAPI.Automation.OpcDaServer> OpcDaServers
		{
			get { return _opcDaServers; }
			set 
			{
				_opcDaServers = value;
				OnPropertyChanged(() => OpcDaServers);
			}
		}

		RubezhAPI.Automation.OpcDaServer _selectedOpcDaServer;

		public RubezhAPI.Automation.OpcDaServer SelectedOpcDaServer
		{
			get { return _selectedOpcDaServer; }
			set
			{
				_selectedOpcDaServer = value;
				Tags = _selectedOpcDaServer == null ? null : _selectedOpcDaServer.Tags;
				OnPropertyChanged(() => SelectedOpcDaServer);
			}
		}

		public RubezhAPI.Automation.OpcDaTag[] Tags 
		{
			get { return SelectedOpcDaServer == null ? null : SelectedOpcDaServer.Tags; }
			set 
			{
				if (SelectedOpcDaServer != null)
				{
					SelectedOpcDaServer.Tags = value;
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
			//Cоздаём и передаём список отсутствующих в конфигурационном списке серверов
			var list = OpcDaServer.OpcDaServer.GetRegistredServers().Select(x =>
				new RubezhAPI.Automation.OpcDaServer { Id = x.Id, ServerName = x.ServerName }).ToList();

			var notselectedServers = list.Where(x => !OpcDaServers.Any(y => y.Id == x.Id));
			var addingDialog = new OpcDaAddingServersViewModel(notselectedServers);

			if (DialogService.ShowModalWindow(addingDialog))
			{
				foreach (var server in addingDialog.SelectedServers)
				{
					OpcDaServers.Add(server);
					ClientManager.SystemConfiguration.AutomationConfiguration
						.OpcDaServers.Add(server);
					ServiceFactory.SaveService.AutomationChanged = true;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			ClientManager.SystemConfiguration.AutomationConfiguration
				.OpcDaServers.Remove(SelectedOpcDaServer);
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
			var allTags = OpcDaServerHelper.GetAllTagsFromOpcServer(
				OpcDaServer.OpcDaServer.GetRegistredServers().First(x => x.Id == SelectedOpcDaServer.Id))
				.Select(tag => new OpcDaEditingTagsTagViewModel(tag)).ToArray();

			// Получаем список уже выбранных тегов
			// и устанавливаем им признак
			foreach (var x in allTags)
			{ 
				foreach(var y in SelectedOpcDaServer.Tags)
				{
					if (x.Tag.TagId == y.TagId)
					{
						x.IsChecked = true;
					}
				}
			}

			var addingDialog = new OpcDaEditingTagsViewModel(allTags);

			if (DialogService.ShowModalWindow(addingDialog))
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

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnectToServer()
		{
			OpcDaWorkWithServerViewModel vm = 
				new OpcDaWorkWithServerViewModel(SelectedOpcDaServer);
			DialogService.ShowModalWindow(vm);
		}
		bool CanConnectToServer()
		{
			return SelectedOpcDaServer != null;
		}

		#endregion
	}
}