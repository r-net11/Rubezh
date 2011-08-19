using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionZoneViewModel : BaseViewModel
    {
        public InstructionZoneViewModel(Zone zone)
        {
            _instructionZone = new InstructionZone();
            _instructionZone.ZoneNo = zone.No;
        }

        public InstructionZoneViewModel(InstructionZone instructionZone)
        {
            _instructionZone = instructionZone;
        }

        InstructionZone _instructionZone;

        public string ZoneNo
        {
            get { return _instructionZone.ZoneNo; }
            set
            {
                _instructionZone.ZoneNo = value;
                OnPropertyChanged("ZoneNo");
            }
        }

        public string Text
        {
            get { return _instructionZone.Text; }
            set
            {
                _instructionZone.Text = value;
                OnPropertyChanged("Text");
            }
        }
    }
}
