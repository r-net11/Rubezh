using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Common;
using Controls.MessageBox;

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
            Phone1 = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "Phone1").Value);
            Phone2 = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "Phone2").Value);
            Phone3 = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "Phone3").Value);
            Phone4 = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "Phone4").Value);
            ObjectNumber = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "ObjectNumber").Value);
            TestDialtone = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "TestDialtone").Value);
            TestVoltage = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "TestVoltage").Value);
            CountRecalls = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "CountRecalls").Value);
            Timeout = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "Timeout").Value);
            OutcomingTest = StringToInt(Device.Properties.FirstOrDefault(x => x.Name == "OutcomingTest").Value);

            InitializeFilters();
            var eventsFilter = Device.Properties.FirstOrDefault(x => x.Name == "EventsFilter").Value;
            var array = eventsFilter.ToCharArray();
            for (int i = 0; i < array.Count(); i++)
            {
                FilterItems[i].IsActive = array[i] == '1';
            }
        }

        void SaveProperties()
        {
            Device.Properties.FirstOrDefault(x => x.Name == "Phone1").Value = Phone1.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "Phone2").Value = Phone2.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "Phone3").Value = Phone3.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "Phone4").Value = Phone4.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "ObjectNumber").Value = ObjectNumber.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "TestDialtone").Value = TestDialtone.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "TestVoltage").Value = TestVoltage.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "CountRecalls").Value = CountRecalls.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "Timeout").Value = Timeout.ToString();
            Device.Properties.FirstOrDefault(x => x.Name == "OutcomingTest").Value = OutcomingTest.ToString();

            var stringBuilder = new StringBuilder();
            foreach (var filterItem in FilterItems)
            {
                stringBuilder.Append(filterItem.IsActive ? '1' : '0');
            }
            Device.Properties.FirstOrDefault(x => x.Name == "EventsFilter").Value = stringBuilder.ToString();
        }

        int StringToInt(string value)
        {
            try
            {
                return System.Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        public List<FilterItemViewModel> FilterItems { get; private set; }

        int _phone1;
        public int Phone1
        {
            get { return _phone1; }
            set
            {
                _phone1 = value;
                OnPropertyChanged("Phone1");
            }
        }

        int _phone2;
        public int Phone2
        {
            get { return _phone2; }
            set
            {
                _phone2 = value;
                OnPropertyChanged("Phone2");
            }
        }

        int _phone3;
        public int Phone3
        {
            get { return _phone3; }
            set
            {
                _phone3 = value;
                OnPropertyChanged("Phone3");
            }
        }

        int _phone4;
        public int Phone4
        {
            get { return _phone4; }
            set
            {
                _phone4 = value;
                OnPropertyChanged("Phone4");
            }
        }

        int _objectNumber;
        public int ObjectNumber
        {
            get { return _objectNumber; }
            set
            {
                _objectNumber = value;
                OnPropertyChanged("ObjectNumber");
            }
        }

        int _testDialtone;
        public int TestDialtone
        {
            get { return _testDialtone; }
            set
            {
                _testDialtone = value;
                OnPropertyChanged("TestDialtone");
            }
        }

        int _testVoltage;
        public int TestVoltage
        {
            get { return _testVoltage; }
            set
            {
                _testVoltage = value;
                OnPropertyChanged("TestVoltage");
            }
        }

        int _countRecalls;
        public int CountRecalls
        {
            get { return _countRecalls; }
            set
            {
                _countRecalls = value;
                OnPropertyChanged("CountRecalls");
            }
        }

        int _timeout;
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                OnPropertyChanged("Timeout");
            }
        }

        int _outcomingTest;
        public int OutcomingTest
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
            MessageBoxService.Show("Эта функция пока не реализована");
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
