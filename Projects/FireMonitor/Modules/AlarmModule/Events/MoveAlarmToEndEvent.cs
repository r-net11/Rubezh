using AlarmModule.ViewModels;
using Microsoft.Practices.Prism.Events;

namespace AlarmModule.Events
{
    public class MoveAlarmToEndEvent : CompositePresentationEvent<AlarmViewModel>
    {
    }
}