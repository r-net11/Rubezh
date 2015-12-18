using RubezhAPI;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ControlOpcDaTagStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		ControlOpcDaTagArguments ControlOpcDaTagArguments { get; set; }
		public ControlElementType ControlElementType { get; private set; }

		public ControlOpcDaTagStepViewModel(StepViewModel stepViewModel, ControlElementType controlElementType)
			: base(stepViewModel)
		{
			ControlOpcDaTagArguments = stepViewModel.Step.ControlOpcDaTagArguments;
			ControlElementType = controlElementType;
			ValueArgument = new ArgumentViewModel(ControlOpcDaTagArguments.ValueArgument, stepViewModel.Update, UpdateContent, controlElementType == ControlElementType.Set);
		}

		public ObservableCollection<OpcDaServer> OpcDaServers { get; private set; }
		OpcDaServer _selectedOpcDaServer;
		public OpcDaServer SelectedOpcDaServer
		{
			get { return _selectedOpcDaServer; }
			set
			{
				_selectedOpcDaServer = value;
				if (_selectedOpcDaServer != null)
				{
					ControlOpcDaTagArguments.OpcDaServerUID = _selectedOpcDaServer.Uid;
					OpcDaTags = new ObservableCollection<OpcDaTag>(_selectedOpcDaServer.Tags);
				}
				else
					ControlOpcDaTagArguments.OpcDaServerUID = Guid.Empty;
				OnPropertyChanged(() => SelectedOpcDaServer);
			}
		}

		public ObservableCollection<OpcDaTag> OpcDaTags { get; private set; }
		OpcDaTag _selectedOpcDaTag;
		public OpcDaTag SelectedOpcDaTag
		{
			get { return _selectedOpcDaTag; }
			set
			{
				_selectedOpcDaTag = value;
				OnPropertyChanged(() => SelectedOpcDaTag);
			}
		}

		public override void UpdateContent()
		{
			var allServers = new List<OpcDaServer>(ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaServers);
			allServers.AddRange(ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers);
			OpcDaServers = new ObservableCollection<OpcDaServer>(allServers);
			SelectedOpcDaServer = OpcDaServers.FirstOrDefault(x => x.Uid == ControlOpcDaTagArguments.OpcDaServerUID);
			OnPropertyChanged(() => OpcDaServers);
		}

		public override string Description
		{
			get
			{
				return "OPC DA Сервер: " + (SelectedOpcDaServer != null ? SelectedOpcDaServer.ServerName : "<пусто>") +
					"; Тэг: " + (SelectedOpcDaTag != null ? SelectedOpcDaTag.TagName : "<пусто>") +
					"; Операция: " + ControlElementType.ToDescription() +
					"; Значение: " + ValueArgument.Description;
			}
		}
	}
}