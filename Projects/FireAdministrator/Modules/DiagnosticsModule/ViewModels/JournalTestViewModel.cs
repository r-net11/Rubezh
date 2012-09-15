using System;
using System.Collections.Generic;
using System.Windows.Input;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class JournalTestViewModel : DialogViewModel
	{
		#region Construction
		public JournalTestViewModel()
		{
			gke = new GkEvents();
			Query = gke.Select_Items();
		}
		#endregion

		#region Fields
		GkEvents gke;
		List<gkEvent> _query;
		DateTime starting_date = DateTime.Parse("1.1.1900");
		DateTime ending_date = DateTime.Parse("31.12.2012");
        uint starting_gkno = 0;
        uint ending_gkno = 100;
		bool _gkno_chk;
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
		public DateTime Starting_date
		{
			get { return starting_date; }
			set
			{
				starting_date = value;
                OnPropertyChanged("Starting_date");
			}
		}
		public DateTime Ending_date
		{
			get { return ending_date; }
			set
			{
				ending_date = value;
                OnPropertyChanged("Ending_date");
			}
		}
        public uint Starting_gkno
        {
            get { return starting_gkno; }
            set
            {
                starting_gkno = value;
                OnPropertyChanged("Starting_gkno");
            }
        }
        public uint Ending_gkno
        {
            get { return ending_gkno; }
            set
            {
                ending_gkno = value;
                OnPropertyChanged("Ending_gkno");
            }
        }
		public bool Gkno_chk
		{
			get { return _gkno_chk; }
			set
			{
				_gkno_chk = value;
				OnPropertyChanged("Gkno_chk");
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
			Query = gke.Select_Items(starting_gkno, ending_gkno, starting_date, ending_date);
		}

		public ICommand Show_selected_devices_Command { get { return new RelayCommand(Show_selected_devices, delegate() { return true; }); } }

		#endregion
	}
}