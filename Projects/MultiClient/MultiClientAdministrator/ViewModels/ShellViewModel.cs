using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using System.IO;
using MuliclientAPI;
using System.Runtime.Serialization;
using Common;

namespace MultiClient.ViewModels
{
	public class ShellViewModel : BaseViewModel
	{
		public ShellViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			SaveCommand = new RelayCommand(OnSave);
			AppItems = new ObservableCollection<AppItemViewModel>();

			var configuration = LoadData();
			foreach (var multiclientData in configuration.MulticlientDatas)
			{
				var appItemViewModel = new AppItemViewModel()
				{
					Name = multiclientData.Name,
					Address = multiclientData.Address,
					Port = multiclientData.Port,
					Login = multiclientData.Login,
					Password = multiclientData.Password
				};
				AppItems.Add(appItemViewModel);
			}
		}

		public MulticlientConfiguration LoadData()
		{
			try
			{
				EncryptHelper.DecryptFile("Configuration.xml", "TempConfiguration.xml");

				var memStream = new MemoryStream();
				using (var fileStream = new FileStream("TempConfiguration.xml", FileMode.Open))
				{
					memStream.SetLength(fileStream.Length);
					fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
				}
				File.Delete("TempConfiguration.xml");
				var dataContractSerializer = new DataContractSerializer(typeof(MulticlientConfiguration));
				var configuration = (MulticlientConfiguration)dataContractSerializer.ReadObject(memStream);
				if (configuration == null)
					return new MulticlientConfiguration();
				return configuration;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ShellViewModel.LoadData");
			}
			return new MulticlientConfiguration();
		}

		public void SaveData(MulticlientConfiguration configuration)
		{
			try
			{
                using (var memoryStream = new MemoryStream())
                {
					var dataContractSerializer = new DataContractSerializer(typeof(MulticlientConfiguration));
                    dataContractSerializer.WriteObject(memoryStream, configuration);

					using (var fileStream = new FileStream("TempConfiguration.xml", FileMode.Create))
					{
						fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
					}
					EncryptHelper.EncryptFile("TempConfiguration.xml", "Configuration.xml");
					File.Delete("TempConfiguration.xml");
                }
			}
			catch (Exception e)
			{
				Logger.Error(e, "ShellViewModel.SaveData");
			}
		}

		public ObservableCollection<AppItemViewModel> AppItems { get; private set; }

		AppItemViewModel _selectedAppItem;
		public AppItemViewModel SelectedAppItem
		{
			get { return _selectedAppItem; }
			set
			{
				_selectedAppItem = value;
				OnPropertyChanged("SelectedAppItem");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var appItemViewModel = new AppItemViewModel();
			AppItems.Add(appItemViewModel);
			SelectedAppItem = AppItems.LastOrDefault();
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			AppItems.Remove(SelectedAppItem);
			SelectedAppItem = AppItems.FirstOrDefault();
		}
		bool CanRemove()
		{
			return SelectedAppItem != null;
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			var configuration = new MulticlientConfiguration();
			foreach (var appItem in AppItems)
			{
				var multiclientData = new MulticlientData()
				{
					Name = appItem.Name,
					Address = appItem.Address,
					Port = appItem.Port,
					Login = appItem.Login,
					Password = appItem.Password
				};
				configuration.MulticlientDatas.Add(multiclientData);
			}
			SaveData(configuration);
		}
	}
}