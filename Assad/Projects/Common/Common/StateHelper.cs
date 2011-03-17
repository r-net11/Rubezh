using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class StateHelper
    {
        static List<StateClass> StateClasses;

        static StateHelper()
        {
            StateClasses = new List<StateClass>();
            StateClasses.Add(new StateClass() { Id = 0, Name = "Тревога" });
            StateClasses.Add(new StateClass() { Id = 1, Name = "Внимание (предтревожное)" });
            StateClasses.Add(new StateClass() { Id = 2, Name = "Неисправность" });
            StateClasses.Add(new StateClass() { Id = 3, Name = "Требуется обслуживание" });
            StateClasses.Add(new StateClass() { Id = 4, Name = "Обход устройств" });
            StateClasses.Add(new StateClass() { Id = 5, Name = "Неопределено" });
            StateClasses.Add(new StateClass() { Id = 6, Name = "Норма(*)" });
            StateClasses.Add(new StateClass() { Id = 7, Name = "Норма" });
            //StateClasses.Add(new StateClass() { Id = 8, Name = "Нет состояния" });
        }

        public static string GetState(int id)
        {
            return StateClasses.FirstOrDefault(x => x.Id == id).Name;
        }
    }

    class StateClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
