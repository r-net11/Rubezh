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

			Tags = _opcDaServersViewModel.SelectedTags
				.Where(x => (x.AccessRights == TsDaAccessRights.ReadWritable) || 
					(x.AccessRights == TsDaAccessRights.Writable))
				.ToArray();

			ReadTagCommand = new RelayCommand(OnReadTag, CanReadTag);
			WriteTagCommand = new RelayCommand(OnWriteTag, CanWriteTag);
		}

		#endregion

		#region Fields And Properties
		
		OpcDaClientViewModel _opcDaServersViewModel;

		OpcDaServer OpcServer { get { return _opcDaServersViewModel.SelectedOpcServer; } }
		public OpcDaTagValue[] _tagValues;
		public OpcDaTagValue[] TagValues
		{
			get { return _tagValues; }
			private set { _tagValues = value; OnPropertyChanged(() => TagValues); }
		}

		public OpcDaTag[] Tags { get; private set; }

		OpcDaTag _selectedTag;
		public OpcDaTag SelectedTag 
		{ 
			get { return _selectedTag; }
			set { _selectedTag = value; OnPropertyChanged(() => SelectedTag); }
		}

		string _tagValue;
		public string TagValue
		{
			get { return _tagValue; }
			set { _tagValue = value; OnPropertyChanged(() => TagValue); }
		}

		#endregion

		#region Methods
		
		public override void OnClosed()
		{
			base.OnClosed();
		}

		#endregion

		#region Commands

		public RelayCommand ReadTagCommand { get; private set; }
		void OnReadTag()
		{
			OperationResult<OpcDaTagValue[]> result = null;

			WaitHelper.Execute(() =>
			{
				result = ClientManager.FiresecService.ReadOpcDaServerTags(ClientManager.ClientCredentials.ClientUID, OpcServer);
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
			OperationResult<bool> result = null;
			Object value;

			WaitHelper.Execute(() =>
			{
				if (SelectedTag.TypeNameOfValue == typeof(SByte).FullName)
				{
					value = SByte.Parse(_tagValue);
				}
				else
				{
					return;
				}
				result = ClientManager.FiresecService.WriteOpcDaServerTag(
					ClientManager.ClientCredentials.ClientUID, SelectedTag.Uid, value); 
			});
		}
		bool CanWriteTag() 
		{ 
			return SelectedTag != null; 
		}

		#endregion
	}
}