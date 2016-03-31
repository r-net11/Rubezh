using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using RubezhClient;
using Infrastructure.Common.Windows;
using RubezhAPI.Automation;
using Infrastructure;
using System.Text;
using OpcClientSdk;
using RubezhAPI;
using Infrastructure.Events;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientViewModel : MenuViewPartViewModel, IDisposable
	{
		#region Constructors

		public OpcDaClientViewModel()
		{
			Menu = new OpcDaClientMenuViewModel(this);

			AddOpcServerCommand = new RelayCommand(OnAddOpcServer, CanAddOpcServer);
			RemoveOpcServerCommand = new RelayCommand(OnRemoveOpcServer, CanRemoveOpcServer);
			EditTagListCommand = new RelayCommand(OnEditTagList, CanEditTagList);
			GetServerStatusCommand = new RelayCommand(OnGetServerStatus, CanGetServerStatus);
			ReadWriteTagsCommand = new RelayCommand(OnReadWriteTags, CanReadWriteTags);

			RegisterShortcuts();

			LoadConfig();

			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(ConfigurationWasChanged);
		}

		#endregion

		#region Fields And Properties

		ObservableCollection<OpcDaServer> _opcDaServers =
			new ObservableCollection<OpcDaServer>();
		public ObservableCollection<OpcDaServer> OpcDaServers
		{
			get { return _opcDaServers; }
			private set
			{
				_opcDaServers = value;
				OnPropertyChanged(() => OpcDaServers);
			}
		}

		OpcDaServer _selectedOpcServer;
		public OpcDaServer SelectedOpcServer
		{
			get { return _selectedOpcServer; }
			set
			{
				_selectedOpcServer = value;
				OnPropertyChanged(() => SelectedOpcServer);
				SelectedTags = _selectedOpcServer == null ? null : _selectedOpcServer.Tags;
				if (SelectedTags != null)
				{
					foreach (var tag in SelectedTags)
					{
						tag.IsChecked = true;
					}
				}
			}
		}

		//TsOpcTagsStructure _tags;
		//public TsOpcTagsStructure Tags
		//{
		//	get { return _tags; }
		//	set
		//	{
		//		_tags = value;
		//		OnPropertyChanged(() => Tags);
		//	}
		//}

		OpcDaTag[] _selectedTags;
		public OpcDaTag[] SelectedTags
		{
			get { return _selectedTags; }
			set
			{
				_selectedTags = value;
				OnPropertyChanged(() => SelectedTags);
			}
		}

		#endregion

		#region Methods

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddOpcServerCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditTagListCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), RemoveOpcServerCommand);
		}

		void LoadConfig()
		{
			OpcDaServers.Clear();
			if (ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers == null)
			{
				ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers = new List<OpcDaServer>();
			}
			foreach (var opcServer in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers)
			{
				OpcDaServers.Add(opcServer);
			}
		}

		void SaveConfig()
		{
			//throw new NotImplementedException();
		}

		public void Initialize() { }

		public void Dispose() { }

		public void ConfigurationWasChanged(object arg)
		{
			LoadConfig();
		}

		public override void OnShow()
		{
			LoadConfig();
			base.OnShow();
		}

		#endregion

		#region Commands

		public RelayCommand AddOpcServerCommand { get; private set; }
		void OnAddOpcServer()
		{
			var addingServersDialog = new OpcDaClientAddingServersViewModel(this);
			DialogService.ShowModalWindow(addingServersDialog);
		}
		bool CanAddOpcServer() { return OpcDaServers != null; }

		public RelayCommand RemoveOpcServerCommand { get; private set; }
		void OnRemoveOpcServer()
		{
			// Удаляем из фильтров OPC тегов поля связанные с данным сервером
			var tags = SelectedOpcServer.Tags;

			var filters = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.Where(f => tags.Any(t => t.Uid == f.TagUID)).ToArray();

			if (filters.Length > 0)
			{
				var sb = new StringBuilder();
				sb.Append(Environment.NewLine);

				for (int i = 0; i < filters.Length; i++)
				{
					if (i == filters.Length - 1)
						sb.Append("    " + filters[i].Name + ".");
					else
						sb.Append("    " + filters[i].Name + ",");

					sb.Append(Environment.NewLine);
				}

				if (!MessageBoxService.ShowConfirmation(String.Format(
					"При удалении OPC сервера будут изменены фильтры: {0} Продолжить?", sb.ToString()), 
					"Внимание"))
				{
					return;
				}
				else
				{
					foreach (var filter in filters)
						filter.TagUID = Guid.Empty;
				}
			}

			// Удаляем сервер
			ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.Remove(SelectedOpcServer);
			OpcDaServers.Remove(SelectedOpcServer);

			ServiceFactory.SaveService.AutomationChanged = true;
			SelectedOpcServer = OpcDaServers.FirstOrDefault();
		}
		bool CanRemoveOpcServer() { return SelectedOpcServer != null; }

		public RelayCommand EditTagListCommand { get; private set; }
		void OnEditTagList()
		{
			OpcDaClientEditingTagsViewModel editingTagList;

			var tagsBefor = SelectedTags;

			try
			{
				editingTagList = new OpcDaClientEditingTagsViewModel(this);
			}
			catch (Exception ex)
			{
				MessageBoxService.ShowError(ex.Message, "Ошибка OPC сервера"); 
				return;
			}
			
			DialogService.ShowModalWindow(editingTagList);

			if (tagsBefor.Length == SelectedTags.Length)
			{
				var different = tagsBefor.Where(tag => !SelectedTags.Any(t => t.Uid == tag.Uid));
				if (different.Count() == 0)
					return;
			}

			var server = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers
				.FirstOrDefault(x => x == SelectedOpcServer);
			
			var removedTags = tagsBefor.Where(t => !SelectedTags.Contains(t, new OpcDaTagComparer()));

			var changedFilters = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.Where(filter => removedTags.Any(tag => tag.Uid == filter.TagUID)).ToArray();

			if ((removedTags.Count() > 0) && (changedFilters.Count() > 0))
			{
				var sb = new StringBuilder();

				sb.Append(Environment.NewLine);

				for (int i = 0; i < changedFilters.Length; i++)
				{
					if (i == changedFilters.Length - 1)
						sb.Append("    " + changedFilters[i].Name + ".");
					else
						sb.Append("    " + changedFilters[i].Name + ",");

					sb.Append(Environment.NewLine);
				}

				if (!MessageBoxService.ShowConfirmation(String.Format(
					"При удалении OPC тегов будут изменены фильтры: {0} Продолжить?", sb.ToString()),
					"Внимание"))
				{
					SelectedTags = tagsBefor;
					return;
				}
				else
				{
					foreach (var item in changedFilters)
					{
						item.TagUID = Guid.Empty;
					}
					server.Tags = SelectedTags;
				}

				ServiceFactory.SaveService.AutomationChanged = true;
			}
			else
			{
				server.Tags = SelectedTags;
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
		bool CanEditTagList() { return SelectedOpcServer != null; }

		public RelayCommand GetServerStatusCommand { get; private set; }
		void OnGetServerStatus()
		{
			string statusInfo;
			OperationResult<OpcServerStatus> result = null;

			//WaitHelper.Execute(() =>
			//{
			//	resultConnect = ClientManager.FiresecService
			//		.ConnectToOpcDaServer(SelectedOpcServer);
			//});

			//if (resultConnect.HasError)
			//{
			//	statusInfo = string.Format("Ошибка: {0}", resultConnect.Error);
			//}
			//else
			{
				WaitHelper.Execute(() =>
				{
					result = ClientManager.FiresecService
						.GetOpcDaServerStatus(FiresecServiceFactory.UID, SelectedOpcServer);
				});

				if (result.HasError)
				{
					statusInfo = string.Format("Ошибка: {0}", result.Error);
				}
				else
				{
					var sb = new StringBuilder();

					sb.Append("Производитель: ");
					sb.Append(result.Result.VendorInfo);
					sb.Append(Environment.NewLine);

					sb.Append("Статус: ");
					sb.Append(result.Result.ServerState.ToString());
					sb.Append(Environment.NewLine);

					sb.Append("Версия ПО: ");
					sb.Append(result.Result.ProductVersion);
					sb.Append(Environment.NewLine);

					sb.Append("Дата и время на сервере: ");
					sb.Append(result.Result.CurrentTime.ToLocalTime());
					sb.Append(Environment.NewLine);

					sb.Append("Дата и время запуска сервера: ");
					sb.Append(result.Result.StartTime.ToLocalTime());
					sb.Append(Environment.NewLine);

					sb.Append("Дата и время последней связи с клиентом: ");
					sb.Append(result.Result.LastUpdateTime.ToLocalTime());
					sb.Append(Environment.NewLine);

					statusInfo = sb.ToString();
				}

				//var resultDisconnect = ClientManager.FiresecService
				//	.DisconnectFromOpcDaServer(SelectedOpcServer);
			}

			MessageBoxService.Show(statusInfo, "Состояние OPC сервера");
		}
		bool CanGetServerStatus() { return SelectedOpcServer != null; }

		public RelayCommand ReadWriteTagsCommand { get; private set; }
		void OnReadWriteTags()
		{
			var readWriteWindow = new OpcDaClientReadWriteTagsViewModel(this);
			DialogService.ShowModalWindow(readWriteWindow);
		}
		bool CanReadWriteTags() { return SelectedOpcServer != null; }

		#endregion
	}
}