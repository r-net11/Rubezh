using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using System.Windows.Threading;
using RubezhDevices;
using System.Windows.Input;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DeviceEditor
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel()
        {
            Current = this;
        }

        public static DeviceViewModel Current { get; private set; }
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

        public bool dispatcherTimerStop;
        public bool DispatcherTimerStop
        {
            get { return dispatcherTimerStop; }
            set
            {
                dispatcherTimerStop = value;
                dispatcherTimer.Stop();
                OnPropertyChanged("DispatcherTimerStop");
            }
        }

        string cadr1, cadr2;
        int cadrDuration1 = 100, cadrDuration2 = 100;
        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        StateViewModel selectedStateViewModel;
        bool DispatcherTimerTickOn = false;
        public StateViewModel SelectedStateViewModel
        {
            get { return selectedStateViewModel; }
            set
            {
                selectedStateViewModel = value;
                if (selectedStateViewModel == null)
                    return;
                cadr1 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.CadrViewModels[0].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);                
                cadrDuration1 = selectedStateViewModel.CadrViewModels[0].Duration;
                dispatcherTimer.Stop();
                if (selectedStateViewModel.CadrViewModels.Count > 1)
                {
                    cadr2 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.CadrViewModels[1].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    cadrDuration2 = selectedStateViewModel.CadrViewModels[1].Duration;
                    if (!DispatcherTimerTickOn)
                    {
                        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                        DispatcherTimerTickOn = true;
                    }
                    dispatcherTimer.Start();
                }
                else
                {
                    StringReader stringReader = new StringReader(cadr1);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    StateViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                }
                OnPropertyChanged("SelectedStateViewModel");
            }
        }
        public ObservableCollection<StateViewModel> stateViewModels;
        public ObservableCollection<StateViewModel> StateViewModels
        {
            get { return stateViewModels; }
            set
            {
                stateViewModels = value;
                OnPropertyChanged("StateViewModels");
            }
        }

        bool tick = false;
        /****************Таймер*****************/
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            tick = !tick;
            if (tick)
            {
                try
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, cadrDuration1);
                    cadr1 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.CadrViewModels[0].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    StringReader stringReader = new StringReader(cadr1);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    StateViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                }
                catch {}
            }

            else
            {
                try
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, cadrDuration2);
                    cadr2 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.CadrViewModels[1].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    StringReader stringReader = new StringReader(cadr2);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    StateViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);

                }
                catch {}
            }            
        }
    }
}
