using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Client.Plans;
using Infrustructure.Plans.Designer;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Infrustructure.Plans.InstrumentAdorners;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Designer.ViewModels
{
    public class InstrumentGroupViewModel : BaseViewModel, IInstrument
    {
        private bool _lock;

        public InstrumentGroupViewModel(InstrumentGroup group, int index)
        {
            _lock = false;
            Group = group;
            Index = index;
            IsActive = false;
            Instruments = new ObservableCollection<IInstrument>();
            Instruments.CollectionChanged += Instruments_CollectionChanged;
        }

        public InstrumentGroup Group { get; private set; }
        public int Index { get; private set; }
        
        private ObservableCollection<IInstrument> _instruments;
        public ObservableCollection<IInstrument> Instruments
        {
            get { return _instruments; }
            private set
            {
                _instruments = value;
                OnPropertyChanged(() => Instruments);
            }
        }

        private IInstrument _activeInstrument;
        public IInstrument ActiveInstrument
        {
            get { return _activeInstrument; }
            set
            {
                //if (ActiveInstrument != null && ActiveInstrument.Adorner != null)
                //    ActiveInstrument.Adorner.Hide();
                _activeInstrument = value;
                OnPropertyChanged(() => ActiveInstrument);
                //if (ActiveInstrument != null && ActiveInstrument.Autostart)
                //    ApplicationService.BeginInvoke(() => Apply(null));
                if (ActiveInstrument != null)
                {
                    ImageSource = ActiveInstrument.ImageSource;
                    ToolTip = ActiveInstrument.ToolTip;
                    Command = ActiveInstrument.Command;
                    Adorner = ActiveInstrument.Adorner;
                    Autostart = ActiveInstrument.Autostart;
                }
            }
        }

        private string _imageSource;
        public string ImageSource
        {
            get { return _imageSource; }
            private set
            {
                _imageSource = value;
                OnPropertyChanged(() => ImageSource);
            }
        }

        private string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            private set
            {
                _toolTip = value;
                OnPropertyChanged(() => ToolTip);
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            private set
            {
                _isActive = value;
                OnPropertyChanged(() => IsActive);
            }
        }

        public ICommand Command { get; private set; }
        public InstrumentAdorner Adorner { get; private set; }
        public bool Autostart { get; private set; }

        private void Instruments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_lock)
            {
                _lock = true;
                IsActive = Instruments.Count > 0;
                if (IsActive)
                {
                    var sortedItems = Instruments.OrderBy(item => item.Index);
                    int index = 0;
                    foreach (var item in sortedItems)
                    {
                        Instruments.Move(Instruments.IndexOf(item), index);
                        index++;
                    }
                    ActiveInstrument = Instruments[0];
                }
                else
                    ActiveInstrument = null;
                _lock = false;
            }
        }
    }
}
