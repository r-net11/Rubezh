using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class CustomDataViewModel : BaseViewModel
	{
		const string msg = "{0} пользовательских данных - {1}";

		public CustomDataViewModel()
		{
			ReadCustomDataCommand = new RelayCommand(OnReadCustomData);
			WriteCustomDataCommand = new RelayCommand(OnWriteCustomData);
		}
		public RelayCommand ReadCustomDataCommand { get; private set; }
		private void OnReadCustomData()
		{
			Data = MainViewModel.Wrapper.GetCustomData();
			var msgStatus = Data != null ? "Успех" : "Ошибка";
			MessageBox.Show(String.Format(msg, "Чтение", msgStatus));
		}

		public RelayCommand WriteCustomDataCommand { get; private set; }
		private void OnWriteCustomData()
		{
			var msgStatus = MainViewModel.Wrapper.SetCustomData(Data) ? "Успех" : "Ошибка";
			MessageBox.Show(String.Format(msg, "Запись", msgStatus));
		}

		string _data;
		public string Data
		{
			get { return _data; }
			set
			{
				_data = value;
				OnPropertyChanged(() => Data);
			}
		}
	}
}
