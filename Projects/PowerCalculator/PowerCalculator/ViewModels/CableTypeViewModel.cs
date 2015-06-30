using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerCalculator.ViewModels
{
    public class CableTypeViewModel : BaseViewModel
    {
        public CableType CableType { get; private set; }

        public CableTypeViewModel(CableType cableType)
        {
            CableType = cableType;
            _name = CableType.Name;
            _resistivity = CableType.Resistivity;
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
                CableType.Name = _name;
            }
        }

        double _resistivity;
        public double Resistivity
        {
            get { return _resistivity; }
            set
            {
                _resistivity = value;
                OnPropertyChanged(() => Resistivity);
                CableType.Resistivity = _resistivity;
            }
        }
        
    }
}
