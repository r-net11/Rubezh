using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class MS34DetailsViewModel : SaveCancelDialogViewModel
	{
		Device Device;

		public MS34DetailsViewModel(Device device)
		{
			Title = "Параметры устройства: МС";
			Device = device;
			ReadCommand = new RelayCommand(OnRead);
			SetAllCommand = new RelayCommand(OnSetAll);
			ResetAllCommand = new RelayCommand(OnResetAll);

			InitializeFilters();
			var eventsFilterString = Device.Properties.FirstOrDefault(x => x.Name == "EventsFilter");
			if (eventsFilterString != null)
			{
				var eventsFilter = eventsFilterString.Value;
				var array = eventsFilter.ToCharArray();
				for (int i = 0; i < array.Count(); i++)
				{
					FilterItems[i].IsActive = array[i] == '1';
				}
			}
		}

		void InitializeFilters()
		{
			FilterItems = new List<FilterItemViewModel>();
			FilterItems.Add(new FilterItemViewModel("Пожарная тревога"));
			FilterItems.Add(new FilterItemViewModel("Тревога: дымовой датчик"));
			FilterItems.Add(new FilterItemViewModel("Тревога: возгорания"));
			FilterItems.Add(new FilterItemViewModel("Тревога: прорыв воды"));
			FilterItems.Add(new FilterItemViewModel("Тревога: датчик температуры"));
			FilterItems.Add(new FilterItemViewModel("Нажата кнопка ПОЖАР"));
			FilterItems.Add(new FilterItemViewModel("Неисправность трубопровода"));
			FilterItems.Add(new FilterItemViewModel("Тревога: датчик пламени"));
			FilterItems.Add(new FilterItemViewModel("Вероятная пожарная тревога"));
			FilterItems.Add(new FilterItemViewModel("Тревога: вскрытие корпуса"));
			FilterItems.Add(new FilterItemViewModel("Тревога: обрыв шины"));
			FilterItems.Add(new FilterItemViewModel("Тревога: КЗ шины"));
			FilterItems.Add(new FilterItemViewModel("Тревога: отказ модуля расширения"));
			FilterItems.Add(new FilterItemViewModel("Тревога: вскрытие модуля расширения"));
			FilterItems.Add(new FilterItemViewModel("Неудачный опрос датчиков"));
			FilterItems.Add(new FilterItemViewModel("Неисправность системы пожаротушения"));
			FilterItems.Add(new FilterItemViewModel("Тревога: низкое давление воды для пожаротушения"));
			FilterItems.Add(new FilterItemViewModel("Тревога: низкая концентрация CO2 для пожаротушения"));
			FilterItems.Add(new FilterItemViewModel("Тревога: датчик вентеля пожаротушения"));
			FilterItems.Add(new FilterItemViewModel("Тревога: низкий уровень воды для пожаротушения"));
			FilterItems.Add(new FilterItemViewModel("Тревога: насос пожаротушения включен"));
			FilterItems.Add(new FilterItemViewModel("Тревога: неисправность насоса пожаротушения"));
			FilterItems.Add(new FilterItemViewModel("Отсутствие сетевого питания"));
			FilterItems.Add(new FilterItemViewModel("Изменение программы"));
			FilterItems.Add(new FilterItemViewModel("Ошибка при самотестировании"));
			FilterItems.Add(new FilterItemViewModel("Устройство отключено"));
			FilterItems.Add(new FilterItemViewModel("Программный сброс установщиком"));
			FilterItems.Add(new FilterItemViewModel("Неисправностьсирены/реле"));
			FilterItems.Add(new FilterItemViewModel("Адресная линия оборвана"));
			FilterItems.Add(new FilterItemViewModel("Адресная линия КЗ"));
			FilterItems.Add(new FilterItemViewModel("Неисправность модуля расширения"));
			FilterItems.Add(new FilterItemViewModel("Вскрытие модуля расширения"));
			FilterItems.Add(new FilterItemViewModel("Шлейф обрыв"));
			FilterItems.Add(new FilterItemViewModel("Шлейф КЗ"));
			FilterItems.Add(new FilterItemViewModel("Неисправность связанных зон"));
			FilterItems.Add(new FilterItemViewModel("Неисправность датчика"));
			FilterItems.Add(new FilterItemViewModel("Датчик дыма высокая чувствительность"));
			FilterItems.Add(new FilterItemViewModel("Датчик дыма низкая чувствительность"));
			FilterItems.Add(new FilterItemViewModel("Отказ в доступе"));
			FilterItems.Add(new FilterItemViewModel("Неисправность датчика состояния двери"));
			FilterItems.Add(new FilterItemViewModel("Датчик состояния двери шунтирован"));
			FilterItems.Add(new FilterItemViewModel("Исключение пожарной зоны"));
			FilterItems.Add(new FilterItemViewModel("Пожарный тест"));
			FilterItems.Add(new FilterItemViewModel("Системное время и дата изменены"));
			FilterItems.Add(new FilterItemViewModel("Вход в режим программирования"));
			FilterItems.Add(new FilterItemViewModel("Выход из режима программирования"));
			FilterItems.Add(new FilterItemViewModel("Нажата тревожная кнопка"));
			FilterItems.Add(new FilterItemViewModel("Тревога в охранной зоне"));
			FilterItems.Add(new FilterItemViewModel("Тревога тревога в охранной зоне"));
			FilterItems.Add(new FilterItemViewModel("Постановка/снятие с охраны зоны с ПК"));
			FilterItems.Add(new FilterItemViewModel("Постановка/снятие с охраны зоны с прибора"));
			FilterItems.Add(new FilterItemViewModel("Неудачная постановка"));
			FilterItems.Add(new FilterItemViewModel("Неверный пароль"));
			FilterItems.Add(new FilterItemViewModel("Шлейф неисправен"));
		}

		public List<FilterItemViewModel> FilterItems { get; private set; }

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, "Получение данных с модуля доставки сообщений");
		}
		OperationResult<string> _operationResult;
		void OnPropgress()
		{
			_operationResult = FiresecManager.DeviceGetMDS5Data(Device);
		}
		void OnCompleted()
		{
			if (_operationResult.HasError)
			{
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
				return;
			}
			GetConfiguration(_operationResult.Result);
		}
		public void GetConfiguration(string deviceData)
		{
			if (deviceData != null && deviceData.Length >= 54)
			{
				for (int i = 0; i < 54; i++)
				{
					if (deviceData[i] == '1')
						FilterItems[i].IsActive = true;
				}
			}
			else
			{
				MessageBoxService.ShowError("Ошибка при получении данных");
			}
		}

		public RelayCommand SetAllCommand { get; private set; }
		void OnSetAll()
		{
			FilterItems.ForEach(x => x.IsActive = true);
		}

		public RelayCommand ResetAllCommand { get; private set; }
		void OnResetAll()
		{
			FilterItems.ForEach(x => x.IsActive = false);
		}

		void SaveProperties()
		{
			Device.Properties = new List<Property>();

			var stringBuilder = new StringBuilder();
			foreach (var filterItem in FilterItems)
			{
				stringBuilder.Append(filterItem.IsActive ? '1' : '0');
			}

			var property = new Property()
			{
				Name = "EventsFilter",
				Value = stringBuilder.ToString()
			};
			Device.Properties.Add(property);
		}

		protected override bool Save()
		{
			SaveProperties();
			return base.Save();
		}
	}
}