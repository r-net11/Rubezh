using Controls;
using FiresecAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using FiresecClient;
using GKProcessor;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class CodeDetailsViewModel : SaveCancelDialogViewModel
	{
		public XCode Code { get; private set; }

		public CodeDetailsViewModel(XCode code = null)
		{
			ReadPropertiesCommand = new RelayCommand(OnReadProperties);
			WritePropertiesCommand = new RelayCommand(OnWriteProperties);

			if (code == null)
			{
				Title = "Создать код";
				Code = new XCode();
			}
			else
			{
				Title = "Редактировать код";
				Code = code;
			}

			CopyProperies();
		}

		void CopyProperies()
		{
			Name = Code.Name;
			Password = Code.Password;
			CanSetZone = Code.CanSetZone;
			CanUnSetZone = Code.CanUnSetZone;
		}

		void SaveProperies()
		{
			Code.Name = Name;
			Code.Password = Password;
			Code.CanSetZone = CanSetZone;
			Code.CanUnSetZone = CanUnSetZone;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				if ((_name != null) && (_name.Length > 20))
					_name = _name.Substring(0, 20);
				OnPropertyChanged(() => Name);
			}
		}

		int _password;
		public int Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		bool _canSetZone;
		public bool CanSetZone
		{
			get { return _canSetZone; }
			set
			{
				_canSetZone = value;
				OnPropertyChanged(() => CanSetZone);
			}
		}

		bool _canUnSetZone;
		public bool CanUnSetZone
		{
			get { return _canUnSetZone; }
			set
			{
				_canUnSetZone = value;
				OnPropertyChanged(() => CanUnSetZone);
			}
		}

		public RelayCommand ReadPropertiesCommand { get; private set; }
		void OnReadProperties()
		{
			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var result = FiresecManager.FiresecService.GKGetSingleParameter(Code);
			if (!result.HasError && result.Result != null && result.Result.Count == 2)
			{
				var lowPassword = result.Result[0].Value;
				var hiPassword = result.Result[1].Value;
				Password = hiPassword * 65536 + lowPassword;
			}
			else
			{
				MessageBoxService.ShowError(result.Error);
			}
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand WritePropertiesCommand { get; private set; }
		void OnWriteProperties()
		{
			Code.Name = Name;
			Code.Password = Password;

			DescriptorsManager.Create();
			if (!CompareLocalWithRemoteHashes())
				return;

			var baseDescriptor = ParametersHelper.GetBaseDescriptor(Code);
			if (baseDescriptor != null)
			{
				var result = FiresecManager.FiresecService.GKSetSingleParameter(Code, baseDescriptor.Parameters);
				if (result.HasError)
				{
					MessageBoxService.ShowError(result.Error);
				}
			}
			else
			{
				MessageBoxService.ShowError("Ошибка. Отсутствуют параметры");
			}
		}

		bool CompareLocalWithRemoteHashes()
		{
			if (Code.GkDatabaseParent == null)
			{
				MessageBoxService.ShowError("Код не относится ни к одному ГК");
				return false;
			}

			var result = FiresecManager.FiresecService.GKGKHash(Code.GkDatabaseParent);
			if (result.HasError)
			{
				MessageBoxService.ShowError("Ошибка при сравнении конфигураций. Операция запрещена");
				return false;
			}

			var localHash = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, Code.GkDatabaseParent);
			var remoteHash = result.Result;
			if (GKFileInfo.CompareHashes(localHash, remoteHash))
				return true;
			MessageBoxService.ShowError("Конфигурации различны. Операция запрещена");
			return false;
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.Show("Имя не может быть пустым");
				return false;
			}

			SaveProperies();
			return base.Save();
		}
	}
}