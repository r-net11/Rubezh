using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using System.Windows.Controls;
using RubezhDevices;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Input;
using Firesec.Metadata;
using System.Windows;

namespace DeviceEditor
{
    public class StateViewModel : BaseViewModel
    {

        public StateViewModel()
        {
            Current = this;
            Parent = DeviceViewModel.Current;
            RemoveStateCommand = new RelayCommand(OnRemoveStateCommand);
        }
        public static StateViewModel Current { get; private set; }
        public DeviceViewModel Parent { get; set; }
        
        /// <summary>
        /// Выбранный кадр.
        /// </summary>
        FrameViewModel selectedFrameViewModel;
        public FrameViewModel SelectedFrameViewModel
        {
            get { return selectedFrameViewModel; }
            set
            {
                selectedFrameViewModel = value;
                if (selectedFrameViewModel == null)
                    return;
                OnPropertyChanged("SelectedFrameViewModel");
            }
        }

        /// <summary>
        /// Путь к иконке состояний.
        /// </summary>
        string iconPath = @"C:\Rubezh\Assad\Projects\ActivexDevices\Library\Icons\3.png";
        public string IconPath
        {
            get
            {
                return iconPath;
            }
            set
            {
                iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }

        /// <summary>
        /// Команда удаления состояния.
        /// </summary>
        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveStateCommand(object obj)
        {
            if (this.Id == "Базовый рисунок")
            {
                MessageBox.Show("Невозможно удалить Базовый рисунок");
                return;
            }
            this.Parent.StateViewModels.Remove(this);
            StateViewModel stateViewModel = new StateViewModel();
            stateViewModel.Id = this.Id;
            DeviceViewModel.Current.StatesAvailableListViewModel.Add(stateViewModel);
        }

        /// <summary>
        /// Идентификатор состояния.
        /// </summary>
        public string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Список кадров состояния.
        /// </summary>
        public ObservableCollection<FrameViewModel> frameViewModels;
        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get { return frameViewModels; }
            set
            {
                frameViewModels = value;
                OnPropertyChanged("FrameViewModels");
            }
        }

        bool isAdditional;
        public bool IsAdditional
        {
            get { return isAdditional; }
            set
            {
                isAdditional = value;
                OnPropertyChanged("IsAdditional");
            }
        }

    }
}
