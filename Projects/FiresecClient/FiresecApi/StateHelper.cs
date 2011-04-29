using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firesec
{
    public static class StateHelper
    {
        static List<StateClass> StateClasses;

        static StateHelper()
        {
            StateClasses = new List<StateClass>();
            StateClasses.Add(new StateClass() { Id = 0, Name = "Тревога", StateType=StateType.Alarm });
            StateClasses.Add(new StateClass() { Id = 1, Name = "Внимание (предтревожное)", StateType = StateType.Warning });
            StateClasses.Add(new StateClass() { Id = 2, Name = "Неисправность", StateType = StateType.Failure });
            StateClasses.Add(new StateClass() { Id = 3, Name = "Требуется обслуживание", StateType = StateType.Service });
            StateClasses.Add(new StateClass() { Id = 4, Name = "Обход устройств", StateType = StateType.Off });
            StateClasses.Add(new StateClass() { Id = 5, Name = "Неопределено", StateType = StateType.Unknown });
            StateClasses.Add(new StateClass() { Id = 6, Name = "Норма(*)", StateType = StateType.Info });
            StateClasses.Add(new StateClass() { Id = 7, Name = "Норма", StateType = StateType.Norm });
            StateClasses.Add(new StateClass() { Id = 8, Name = "Нет состояния", StateType = StateType.No });
        }

        public static string GetState(int id)
        {
            return StateClasses.FirstOrDefault(x => x.Id == id).Name;
        }

        public static StateType NameToType(string name)
        {
            return StateClasses.FirstOrDefault(x => x.Name == name).StateType;
        }

        public static string TypeToName(StateType stateType)
        {
            return StateClasses.FirstOrDefault(x => x.StateType == stateType).Name;
        }

        public static int NameToPriority(string name)
        {
            if (name == null)
                return 8;
            return StateClasses.FirstOrDefault(x => x.Name == name).Id;
        }
    }

    class StateClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StateType StateType { get; set; }
    }

    public enum StateType
    {
        Alarm,
        Warning,
        Failure,
        Service,
        Off,
        Unknown,
        Info,
        Norm,
        No
    }
}
