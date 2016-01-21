﻿using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhClient;
using System;
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
					SelectedOpcDaTag = OpcDaTags.FirstOrDefault(x => x.Uid == ControlOpcDaTagArguments.OpcDaTagUID);
				}
				else
					ControlOpcDaTagArguments.OpcDaServerUID = Guid.Empty;
				OnPropertyChanged(() => SelectedOpcDaServer);
			}
		}

		ObservableCollection<OpcDaTag> _opcDaTags;
		public ObservableCollection<OpcDaTag> OpcDaTags
		{
			get { return _opcDaTags; }
			set
			{
				_opcDaTags = value;
				OnPropertyChanged(() => OpcDaTags);
			}
		}
		OpcDaTag _selectedOpcDaTag;
		public OpcDaTag SelectedOpcDaTag
		{
			get { return _selectedOpcDaTag; }
			set
			{
				_selectedOpcDaTag = value;
				if (_selectedOpcDaTag != null)
				{
					ControlOpcDaTagArguments.OpcDaTagUID = _selectedOpcDaTag.Uid;
					var explicitType = OpcDaHelper.GetExplicitType(_selectedOpcDaTag.TypeNameOfValue);
					var defaultExplicitType = explicitType.HasValue ?
						explicitType.Value :
						ControlElementType == ControlElementType.Get ? ExplicitType.String : ExplicitType.Integer;
					ValueArgument.Update(Procedure, defaultExplicitType, isList: false);
				}
				else
				{
					ControlOpcDaTagArguments.OpcDaTagUID = Guid.Empty;
					ValueArgument.Update(Procedure);
				}
				OnPropertyChanged(() => SelectedOpcDaTag);
			}
		}

		public override void UpdateContent()
		{
			OpcDaServers = new ObservableCollection<OpcDaServer>(ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers);
			SelectedOpcDaServer = OpcDaServers.FirstOrDefault(x => x.Uid == ControlOpcDaTagArguments.OpcDaServerUID);
			OnPropertyChanged(() => OpcDaServers);
		}

		public override string Description
		{
			get
			{
				return "OPC DA Сервер: " + (SelectedOpcDaServer != null ? SelectedOpcDaServer.ServerName : "<пусто>") +
					"; Тэг: " + (SelectedOpcDaTag != null ? SelectedOpcDaTag.ElementName : "<пусто>") +
					"; Операция: " + ControlElementType.ToDescription() +
					"; Значение: " + ValueArgument.Description;
			}
		}
	}
}