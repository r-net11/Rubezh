using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class OpcDaWorkWithServerViewModel : SaveCancelDialogViewModel
	{
		#region Constructors

		public OpcDaWorkWithServerViewModel(RubezhAPI.Automation.OpcDaServer server)
		{
			Title = "Взаимодействие с OPC Сервером";
			try
			{
				ActiveServer = OpcDaServer.OpcDaServer.GetRegistredServers()
					.FirstOrDefault(x => x.Id == server.Id);
				if (ActiveServer != null)
				{
					ActiveServer.Connect();
				}
			}
			catch
			{
 				// Не установленны необходимые компоненты для работы
				ActiveServer = null;
			}
			ServerSettings = server;
			DisconnectFromServerCommand = 
				new RelayCommand(OnDisconnectFromServer, CanDisconnectFromServer);
			ReadTagCommand = new RelayCommand(OnReadTag, CanReadTag);
		}

		#endregion

		#region Fields And Properties
		RubezhAPI.Automation.OpcDaServer ServerSettings { get; set; }

		OpcDaServer.OpcDaServer _activeServer;
		public OpcDaServer.OpcDaServer ActiveServer
		{
			get { return _activeServer; }
			private set
			{
				_activeServer = value;
				OnPropertyChanged(() => ActiveServer);
			}
		}

		List<OpcDaServer.OpcDaTagValue> _values;
		public List<OpcDaServer.OpcDaTagValue> Values
		{
			get { return _values; }
			private set 
			{
				_values = value;
				OnPropertyChanged(() => Values);
			}
		}

		#endregion

		#region Methods

		void ReadTags()
		{
			List<OpcDaServer.OpcDaTagValue> list = new List<OpcDaServer.OpcDaTagValue>();

			foreach (var tag in ServerSettings.Tags)
			{
				var result = ActiveServer.ReadTag(tag.TagId);
				list.Add(result);
			}
			Values = list;
		}

		public override void OnClosed()
		{
			if ((ActiveServer != null) && (ActiveServer.IsConnected))
			{
				ActiveServer.Disconnect();
			}
			base.OnClosed();
		}
		#endregion

		#region Commands

		public RelayCommand DisconnectFromServerCommand { get; private set; }
		void OnDisconnectFromServer()
		{
			ActiveServer.Disconnect();
		}
		bool CanDisconnectFromServer()
		{
			return (ActiveServer != null) && (ActiveServer.IsConnected);
		}

		public RelayCommand ReadTagCommand { get; private set; }
		void OnReadTag()
		{
			ReadTags();
		}
		bool CanReadTag()
		{
			if ((ActiveServer != null) && (ActiveServer.IsConnected))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion
	}
}