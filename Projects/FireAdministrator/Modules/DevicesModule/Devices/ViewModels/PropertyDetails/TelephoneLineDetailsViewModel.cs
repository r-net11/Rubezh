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
	public class TelephoneLineDetailsViewModel : SaveCancelDialogViewModel
	{
		Device Device;

		public TelephoneLineDetailsViewModel(Device device)
		{
			Title = "Параметры УОО-ТЛ";
			SelectAllCommand = new RelayCommand(OnSelectAll);
			DeselectAllCommand = new RelayCommand(OnDeselectAll);
			ReadCommand = new RelayCommand(OnRead);
			ResetToDirectConnectionCommand = new RelayCommand(OnResetToDirectConnection);

			Device = device;
			CopyProperties();
		}

		void InitializeFilters()
		{
			FilterItems = new List<FilterItemViewModel>();
			FilterItems.Add(new FilterItemViewModel() { Name = "Пожарная тревога" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: дымовой датчик" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: возгорания" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: прорыв воды" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: датчик температуры" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Нажата кнопка ПОЖАР" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неисправность трубопровода" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: датчик пламени" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Вероятная пожарная тревога" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: вскрытие корпуса" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: обрыв шины" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: КЗ шины" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: отказ модуля расширения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: вскрытие модуля расширения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неудачный опрос датчиков" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неисправность системы пожаротушения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: низкое давление воды для пожаротушения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: низкая концентрация СО2 для пожаротушения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: датчик вентиля пожаротушения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: низкий уровень воды для пожаротушения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: насос пожаротушения включен" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога: неисправность насоса пожаротушения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Отсутствие сетевого питания" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Изменение программы" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Ошибка при самотестировании" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Устройство отключено" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Программный сброс установщиком" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неисправность сирены/реле" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Адресная линия оборвана" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Адресная линия КЗ" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неисправность модуля расширения" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Вскрытие модуля расширителя" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Шлейф обрыв" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Шлейф КЗ" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неисправность связанных зон" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неисправность датчика" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Датчик дыма высокая чувствительность" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Датчик дыма низкая чувствительность" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Отказ в доступе" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неисправность датчика состояния двери" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Датчик состояния двери шунтирован" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Исключение пожарной зоны" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Пожарный тест" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Системное время и дата изменены" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Вход в режим программирования" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Выход из режима программирования" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Нажата тревожная кнопка" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тревога в охранной зоне" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Тихая тревога в охранной зоне" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Постановка/снятие с охраны зон с ПК" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Постановка/снятие с охраны зон с прибора" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неудачная постановка" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Неверный пароль" });
			FilterItems.Add(new FilterItemViewModel() { Name = "Шлейф неисправен" });
		}

		void CopyProperties()
		{
            Phone1 = StringToString("Phone1");
            Phone2 = StringToString("Phone2");
            Phone3 = StringToString("Phone3");
            Phone4 = StringToString("Phone4");
            IsDirect = StringToBool("IsDirect");
			ObjectNumber = StringToInt("ObjectNumber");
			TestDialtone = StringToInt("TestDialtone");
			TestVoltage = StringToInt("TestVoltage");
			CountRecalls = StringToInt("CountRecalls");
			Timeout = StringToInt("Timeout");
			OutcomingTest = StringToInt("OutcomingTest");
    
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

		void SaveProperties()
		{
			Device.Properties = new List<Property>();

			SaveProperty(Phone1, "Phone1");
			SaveProperty(Phone2, "Phone2");
			SaveProperty(Phone3, "Phone3");
			SaveProperty(Phone4, "Phone4");
            SaveProperty(IsDirect, "IsDirect");
			SaveProperty(ObjectNumber, "ObjectNumber");
			SaveProperty(TestDialtone, "TestDialtone");
			SaveProperty(TestVoltage, "TestVoltage");
			SaveProperty(CountRecalls, "CountRecalls");
			SaveProperty(Timeout, "Timeout");
			SaveProperty(OutcomingTest, "OutcomingTest");

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

		int? StringToInt(string name)
		{
			try
			{
				var value = Device.Properties.FirstOrDefault(x => x.Name == name).Value;
				return System.Convert.ToInt32(value);
			}
			catch
			{
				return null;
			}
		}
        string StringToString(string name)
        {
            try
            {
                var value = Device.Properties.FirstOrDefault(x => x.Name == name).Value;
                return System.Convert.ToString(value);
            }
            catch
            {
                return null;
            }
        }
        bool StringToBool(string name)
        {
            try
            {
                var value = Device.Properties.FirstOrDefault(x => x.Name == name).Value;
                return System.Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }
		void SaveProperty(int? intProperty, string propertyName)
		{
			if (intProperty.HasValue)
			{
				var property = new Property()
				{
					Name = propertyName,
					Value = intProperty.Value.ToString()
				};
				Device.Properties.Add(property);
			}
		}

        void SaveProperty(string stringProperty, string propertyName)
        {
            if (stringProperty != null)
            {
                var property = new Property()
                {
                    Name = propertyName,
                    Value = stringProperty
                };
                Device.Properties.Add(property);
            }
        }

        void SaveProperty(bool boolProperty, string propertyName)
        {
            var property = new Property()
            {
                Name = propertyName,
                Value = boolProperty.ToString()
            };
            Device.Properties.Add(property);
        }

        bool isDirect;
        public bool IsDirect
        {
            get { return isDirect; }
            set
            {
                isDirect = value;
                OnPropertyChanged("IsDirect");
            }
        }

		public List<FilterItemViewModel> FilterItems { get; private set; }

		string _phone1;
		public string Phone1
		{
			get { return _phone1; }
			set
			{
				_phone1 = value;
                if(!IsDirect)
				OnPropertyChanged("Phone1");
			}
		}

        string _phone2;
        public string Phone2
		{
			get { return _phone2; }
			set
			{
				_phone2 = value;
                if (!IsDirect)
				OnPropertyChanged("Phone2");
			}
		}

        string _phone3;
        public string Phone3
		{
			get { return _phone3; }
			set
			{
				_phone3 = value;
                if (!IsDirect)
				OnPropertyChanged("Phone3");
			}
		}

        string _phone4;
        public string Phone4
		{
			get { return _phone4; }
			set
			{
				_phone4 = value;
                if (!IsDirect)
				OnPropertyChanged("Phone4");
			}
		}

		int? _objectNumber;
		public int? ObjectNumber
		{
			get { return _objectNumber; }
			set
			{
				_objectNumber = value;
				OnPropertyChanged("ObjectNumber");
			}
		}

		int? _testDialtone;
		public int? TestDialtone
		{
			get { return _testDialtone; }
			set
			{
				_testDialtone = value;
                if (!IsDirect)
				OnPropertyChanged("TestDialtone");
			}
		}

		int? _testVoltage;
		public int? TestVoltage
		{
			get { return _testVoltage; }
			set
			{
				_testVoltage = value;
				OnPropertyChanged("TestVoltage");
			}
		}

		int? _countRecalls;
		public int? CountRecalls
		{
			get { return _countRecalls; }
			set
			{
				_countRecalls = value;
				OnPropertyChanged("CountRecalls");
			}
		}

		int? _timeout;
		public int? Timeout
		{
			get { return _timeout; }
			set
			{
				_timeout = value;
				OnPropertyChanged("Timeout");
			}
		}

		int? _outcomingTest;
		public int? OutcomingTest
		{
			get { return _outcomingTest; }
			set
			{
				_outcomingTest = value;
				OnPropertyChanged("OutcomingTest");
			}
		}

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			FilterItems.ForEach(x => x.IsActive = true);
		}

		public RelayCommand DeselectAllCommand { get; private set; }
		void OnDeselectAll()
		{
			FilterItems.ForEach(x => x.IsActive = false);
		}

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

        private string tempPhone1;
        private string tempPhone2;
        private string tempPhone3;
        private string tempPhone4;
        private int? tempTestDialtone;
		public RelayCommand ResetToDirectConnectionCommand { get; private set; }
        void OnResetToDirectConnection()
		{
            
            if (IsDirect)
            {
                tempPhone1 = Phone1;
                tempPhone2 = Phone2;
                tempPhone3 = Phone3;
                tempPhone4 = Phone4;
                tempTestDialtone = TestDialtone;
                Phone1 = "DL";
                Phone2 = null;
                Phone3 = null;
                Phone4 = null;
                TestDialtone = null;
            }
            else
            {
                Phone1 = tempPhone1;
                Phone2 = tempPhone2;
                Phone3 = tempPhone3;
                Phone4 = tempPhone4;
                TestDialtone = tempTestDialtone; 
            }
		}

		protected override bool Save()
		{
			SaveProperties();
			return base.Save();
		}

        public void GetConfiguration(string DeviceData)
        {
            // TESTMODE
            // var DeviceData = "0                    0                    0                    0                    00000212010100000000111001100000000000000000000000000000000000000000000000";
            string result = "";
            result = result.PadRight(result.Length, '0');
            string s;
            bool b = true; // if Device.DeviceDriver.GetBaseType = 102
            if (b)
            {
                s = DeviceData.Substring(0, 21);
                s = DeleteCharsFromEnd(s, ' ');
                Phone1 = s;

                s = DeviceData.Substring(21, 21);
                s = DeleteCharsFromEnd(s, ' ');
                Phone2 = s;

                s = DeviceData.Substring(42, 21);
                s = DeleteCharsFromEnd(s, ' ');
                Phone3 = s;

                s = DeviceData.Substring(63, 21);
                s = DeleteCharsFromEnd(s, ' ');
                Phone4 = s;

                ObjectNumber = int.Parse(DeviceData.Substring(84, 4));
                TestDialtone = int.Parse(DeviceData.Substring(88, 2)) * 5;
                TestVoltage = int.Parse(DeviceData.Substring(90, 1)) * 10;
                CountRecalls = int.Parse(DeviceData.Substring(91, 1));
                Timeout = int.Parse(DeviceData.Substring(92, 1)) * 10;
                OutcomingTest = int.Parse(DeviceData.Substring(93, 1)) * 10;
                for (int i = 0; i < 54; i++)
                {
                    if (DeviceData[94 + i] == '1')
                        FilterItems[i].IsActive = true;
                }
            }
            else
                for (int i = 0; i < 54; i++)
                {
                    if (DeviceData[i] == '1')
                        FilterItems[i].IsActive = true;
                }
        }
        static int BinStrToByte(string str)
        {
            int result = 0;
            if (str.Length == 8)
            {
                if (str[0] == '1')
                    result = result + 128;
                if (str[1] == '1')
                    result = result + 64;
                if (str[2] == '1')
                    result = result + 32;
                if (str[3] == '1')
                    result = result + 16;
                if (str[4] == '1')
                    result = result + 8;
                if (str[5] == '1')
                    result = result + 4;
                if (str[6] == '1')
                    result = result + 2;
                if (str[7] == '1')
                    result = result + 1;
            }
            return result;
        }

        static string DeleteCharsFromEnd(string str, char ch)
        {
            int cnt = 0;
            for (int i = str.Length - 1; i > 0; i--)
            {
                if (str[i] == ch)
                    cnt++;
                else
                    break;
            }
            if (cnt > 0)
                str = str.Remove(str.Length - cnt, cnt);
            return str;
        }
	}

	public class FilterItemViewModel : BaseViewModel
	{
		public string Name { get; set; }

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged("IsActive");
			}
		}
	}


}