using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common;
using DeviceLibrary;
using Firesec.Metadata;

namespace DeviceEditor.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        private ObservableCollection<FrameViewModel> _frameViewModels;
        private string _iconPath = @"C:\Rubezh\Projects\FireAdministrator\ActivexDevices\Library\states.png";
        private string _id;
        private bool _isAdditional;
        private bool _isChecked;
        private string _name;
        private FrameViewModel _selectedFrameViewModel;
        public StateViewModel()
        {
            Current = this;
            ParentDevice = DeviceViewModel.Current;
            RemoveStateCommand = new RelayCommand(OnRemoveStateCommand);
        }
        public static StateViewModel Current { get; private set; }
        /// <summary>
        /// Родительское для данного состояния устройство.
        /// </summary>
        public DeviceViewModel ParentDevice { get; set; }
        /// <summary>
        /// Выбранный кадр.
        /// </summary>
        public FrameViewModel SelectedFrameViewModel
        {
            get { return _selectedFrameViewModel; }
            set
            {
                _selectedFrameViewModel = value;
                OnPropertyChanged("SelectedFrameViewModel");
            }
        }
        /// <summary>
        /// Флаг. Если true -> дополнительное состояние выбранo.
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                if (_isChecked)
                {
                    ViewModel.Current.SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel.Add(Id);
                }
                if (!_isChecked)
                {
                    ViewModel.Current.SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel.Remove(Id);
                }
                ViewModel.Current.SelectedStateViewModel.ParentDevice.DeviceControl.AdditionalStatesIds =
                    ViewModel.Current.SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel;
                OnPropertyChanged("IsChecked");
            }
        }
        /// <summary>
        /// Путь к иконке состояний.
        /// </summary>
        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                _iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }
        /// <summary>
        /// Команда удаления состояния.
        /// </summary>
        public RelayCommand RemoveStateCommand { get; private set; }
        private void OnRemoveStateCommand(object obj)
        {
            if (!IsAdditional)
            {
                MessageBox.Show("Невозможно удалить основное состояние");
                return;
            }
            else
            {
                IsChecked = false;
            }
            ParentDevice.StatesViewModel.Remove(this);
        }
        /// <summary>
        /// Идентификатор состояния.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                drvType driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == ParentDevice.Id);
                if (IsAdditional)
                    Name = driver.state.FirstOrDefault(x => x.id == _id).name;
                else
                    Name = LibraryManager.BaseStatesList[Convert.ToInt16(_id)];
            }
        }
        /// <summary>
        /// Название состояния.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// Список кадров состояния.
        /// </summary>
        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get { return _frameViewModels; }
            set
            {
                _frameViewModels = value;
                OnPropertyChanged("FrameViewModels");
            }
        }
        /// <summary>
        /// Флаг. Если true -> состояние дополнительное, иначе - основное.
        /// </summary>
        public bool IsAdditional
        {
            get { return _isAdditional; }
            set
            {
                _isAdditional = value;
                OnPropertyChanged("IsAdditional");
            }
        }
    }
}