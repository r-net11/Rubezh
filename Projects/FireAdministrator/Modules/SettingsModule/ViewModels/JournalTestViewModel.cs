using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Input;
using Infrastructure.Common;
using Common.GK;
using Common.GK.Journal;


namespace SettingsModule.ViewModels
{
    public class JournalTestViewModel:DialogViewModel
    {
        
        #region Construction
        public JournalTestViewModel()
        {
            gke = new GkEvents();
            Query = gke.Select_Items();
            selected_devices = new List<string>();
        }
        #endregion

        #region Fields
        GkEvents gke;
        List<gkEvent> _query;
        List<string> selected_devices;
        DateTime starting_date = DateTime.Parse("1.1.1900");
        DateTime ending_date = DateTime.Parse("31.12.2012");
        bool _ip_chk;
        bool _rm_chk;
        bool _mdu_chk;
        bool _mpt_chk;
        bool _device_chk;
        bool _date_chk;
        #endregion

        #region Properties
        public List<gkEvent> Query
        {
            get { return _query; }
            set
            {
                if (_query != value)
                {
                    _query = value;
                    OnPropertyChanged("Query");
                }
            }
        }
        public List<string> Selected_divices
        {
            get { return selected_devices; }
            set
            {
                selected_devices = value;
            }
        }
        public DateTime Starting_date
        {
            get { return starting_date; }
            set
            {
                starting_date = value;
            }
        }
        public DateTime Ending_date
        {
            get { return ending_date; }
            set
            {
                ending_date = value;
            }
        }
        public bool Ip_chk
        {
            get { return _ip_chk; }
            set { _ip_chk = value; }
        }
        public bool Rm_chk
        {
            get { return _rm_chk; }
            set { _rm_chk = value; }
        }
        public bool Mdu_chk
        {
            get { return _mdu_chk; }
            set { _mdu_chk = value; }
        }
        public bool Mpt_chk
        {
            get { return _mpt_chk; }
            set { _mpt_chk = value; }
        }
        public bool Device_chk
        {
            get { return _device_chk; }
            set
            {
                _device_chk = value;
                OnPropertyChanged("Device_chk");
            }
        }
        public bool Date_chk
        {
            get { return _date_chk; }
            set
            {
                _date_chk = value;
                OnPropertyChanged("Date_chk");
            }
        }
        #endregion

        #region Methods
        void GetDeviceFilter()
        {
            if (_ip_chk)
                Selected_divices.Add("IP");
            if (_rm_chk)
                Selected_divices.Add("RM");
            if (_mdu_chk)
                Selected_divices.Add("MDU");
            if (_mpt_chk)
                Selected_divices.Add("MPT");
        }

        private bool valid_dates()
        {
            return starting_date != null &&
                   ending_date != null &&
                   starting_date < ending_date;
        }
        #endregion

        #region Commands

        void Show_selected_devices()
        {
            //if (Device_chk && !Date_chk)
            //{
            //    GetDeviceFilter();
            //    Query = gke.Select_Items(selected_devices);
            //}
            //else if (!Device_chk && Date_chk && valid_dates())
            //{
            //    Query = gke.Select_Items(starting_date, ending_date);
            //}
            //else if (Device_chk && Date_chk && valid_dates())
            //{
            //    GetDeviceFilter();
            //    Query = gke.Select_Items(selected_devices, starting_date, ending_date);
            //}
            //else
                Query = gke.Select_Items();
        }
        
        public ICommand Show_selected_devices_Command { get { return new RelayCommand(Show_selected_devices, delegate() { return true; }); } }
        
        #endregion
        
 



    }
}
