using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using Infrastructure;
using RubezhClient;

namespace AutomationModule.ViewModels
{
	public class OpcDaServersViewModel : MenuViewPartViewModel
	{
		#region Constructors

		public OpcDaServersViewModel()
		{
			Menu = new OpcDaServersMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			OpcDaServers = new ObservableCollection<OpcDaServerViewModel>();

			// Загрузка сохранённого списка серверов
			foreach (var opcServer in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaServers)
			{
				var opcServerViewModel = new OpcDaServerViewModel(opcServer);
				OpcDaServers.Add(opcServerViewModel);
			}
		}

		#endregion

		#region Fields And Properties

		ObservableCollection<OpcDaServerViewModel> _opcDaServers;

		public ObservableCollection<OpcDaServerViewModel> OpcDaServers
		{
			get { return _opcDaServers; }
			set 
			{
				_opcDaServers = value;
				OnPropertyChanged(() => OpcDaServers);
			}
		}

		OpcDaServerViewModel _selectedOpcDaServer;

		public OpcDaServerViewModel SelectedOpcDaServer
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
				SelectedOpcDaServer.Server.Tags = value; 
				OnPropertyChanged(() => Tags); 
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
			var list = OpcDaServer.OpcDaServer.GetRegistredServers();
			var notselectedServers = OpcDaServers.Count == 0 ?
									list.Select(x => new OpcDaServerViewModel(
										new RubezhAPI.Automation.OpcDaServer { Id = x.Id, ServerName= x.ServerName })) :
									from rs in list
									from ss in OpcDaServers
									where rs.Id != ss.Id
									select new OpcDaServerViewModel(
										new RubezhAPI.Automation.OpcDaServer { Id = rs.Id, ServerName = rs.ServerName });

			var addingDialog = new OpcDaServersAddingServersViewModel(notselectedServers);

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
			var tagsList = from a in allTags
						   from s in SelectedOpcDaServer.Server.Tags
						   where a.Tag.TagId == s.TagId
						   select a;
			foreach (var tag in tagsList)
			{
				tag.IsChecked = true;
			}

			var addingDialog = new OpcDaEditingTagsViewModel(allTags);

			if (Infrastructure.Common.Windows.DialogService.ShowModalWindow(addingDialog))
			{
				Tags = (addingDialog.SelectedItems
					.Select(x => x.Tag)).ToArray();
				//ClientManager.SystemConfiguration.AutomationConfiguration
				//	.OpcDaServers.Add(SelectedOpcDaServer.Server);
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