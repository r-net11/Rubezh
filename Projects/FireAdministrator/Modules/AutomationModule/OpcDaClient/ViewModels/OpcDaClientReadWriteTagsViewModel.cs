using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcClientSdk.Da;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientReadWriteTagsViewModel : SaveCancelDialogViewModel
	{
		#region Constructors

		public OpcDaClientReadWriteTagsViewModel(OpcDaClientViewModel vm)
		{
			Title = "Запись и чтение тегов";
			_opcDaServersViewModel = vm;

			ReadTagCommand = new RelayCommand(OnReadTag, CanReadTag);

			WaitHelper.Execute(ConnectToServer);
		}

		#endregion

		#region Fields And Properties
		
		OpcDaClientViewModel _opcDaServersViewModel;

		OpcDaServer OpcServer { get { return _opcDaServersViewModel.SelectedOpcServer; } }
		public TsCDaItemValueResult[] _tagValues;
		public TsCDaItemValueResult[] TagValues
		{
			get { return _tagValues; }
			private set { _tagValues = value; OnPropertyChanged(() => TagValues); }
		}

		#endregion

		#region Methods
		
		void ConnectToServer()
		{
			//var result = ClientManager.FiresecService.ConnectToOpcDaServer(OpcServer);

			//if (result.HasError)
			//{
			//	MessageBoxService.Show(string.Format(
			//		"Ошибка при подключении к серверу: ", result.Error));
			//	Close(false); // Завершаем работу окна
			//}
		}

		void DisconnectFromServer()
		{
			//var result = ClientManager.FiresecService.DisconnectFromOpcDaServer(OpcServer);

			//if (result.HasError)
			//{
			//	MessageBoxService.Show(string.Format(
			//		"Ошибка при отключении от сервера: ", result.Error));
			//}
		}

		public override void OnClosed()
		{
			WaitHelper.Execute(DisconnectFromServer);
			base.OnClosed();
		}

		#endregion

		#region Commands

		public RelayCommand ReadTagCommand { get; private set; }
		void OnReadTag()
		{
			OperationResult<TsCDaItemValueResult[]> result = null;

			WaitHelper.Execute(() =>
			{
				result = ClientManager.FiresecService.ReadOpcDaServerTags(FiresecServiceFactory.UID, OpcServer);
			});

			if (!result.HasError)
			{
				TagValues = result.Result;
			}
			else
			{
				TagValues = null;
			}
		}
		bool CanReadTag() { return OpcServer != null; }

		public RelayCommand WriteTagCommand { get; private set; }
		void OnWriteTag()
		{

		}
		bool CanWriteTag() { return OpcServer != null; }

		#endregion
	}
}