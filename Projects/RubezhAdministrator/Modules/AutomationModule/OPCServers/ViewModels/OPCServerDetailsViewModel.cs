using RubezhAPI.Automation;
using RubezhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class OPCServerDetailsViewModel : SaveCancelDialogViewModel
	{
		public OPCServer OPCServer { get; private set; }

		public OPCServerDetailsViewModel(OPCServer opcServer = null)
		{
			if (opcServer == null)
			{
				OPCServer = new OPCServer();
				Title = "Создание OPC Сервера";
			}
			else
			{
				OPCServer = opcServer;
				Title = "Редактирование OPC Сервера";
			}

			Name = OPCServer.Name;
			Address = OPCServer.Address;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}


		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			OPCServer.Name = Name;
			OPCServer.Address = Address;
			return base.Save();
		}
	}
}