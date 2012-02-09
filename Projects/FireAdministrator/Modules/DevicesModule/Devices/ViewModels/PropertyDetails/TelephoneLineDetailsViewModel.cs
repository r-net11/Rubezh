using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class TelephoneLineDetailsViewModel : SaveCancelDialogContent
    {
        Device Device;

        public TelephoneLineDetailsViewModel(Device device)
        {
            Title = "Параметры МС-ТЛ";
            SelectAllCommand = new RelayCommand(OnSelectAll);
            DeselectAllCommand = new RelayCommand(OnDeselectAll);
            ReadCommand = new RelayCommand(OnRead);

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
        }

        void CopyProperties()
        {
            Phone1 = StringToInt("Phone1");
            Phone2 = StringToInt("Phone2");
            Phone3 = StringToInt("Phone3");
            Phone4 = StringToInt("Phone4");
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

        public List<FilterItemViewModel> FilterItems { get; private set; }

        int? _phone1;
        public int? Phone1
        {
            get { return _phone1; }
            set
            {
                _phone1 = value;
                OnPropertyChanged("Phone1");
            }
        }

        int? _phone2;
        public int? Phone2
        {
            get { return _phone2; }
            set
            {
                _phone2 = value;
                OnPropertyChanged("Phone2");
            }
        }

        int? _phone3;
        public int? Phone3
        {
            get { return _phone3; }
            set
            {
                _phone3 = value;
                OnPropertyChanged("Phone3");
            }
        }

        int? _phone4;
        public int? Phone4
        {
            get { return _phone4; }
            set
            {
                _phone4 = value;
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
            DeviceGetMDS5DataHelper.Run(Device);
        }

        protected override void Save(ref bool cancel)
        {
            SaveProperties();
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
