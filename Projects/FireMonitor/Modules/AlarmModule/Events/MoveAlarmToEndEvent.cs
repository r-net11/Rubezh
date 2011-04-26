using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlarmModule.ViewModels;
using Microsoft.Practices.Prism.Events;

namespace AlarmModule.Events
{
    public class MoveAlarmToEndEvent : CompositePresentationEvent<AlarmViewModel>
    {
    }
}
