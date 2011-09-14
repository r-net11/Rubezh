using System.Collections.Generic;

namespace DeviveModelManager
{
    public class MonitorHelper
    {
        public static TreeItem CreateMonitor()
        {
            var monitor = new TreeItem();
            monitor.Name = "Monitor";
            monitor.ModelInfo = new Assad.modelInfoType()
            {
                name = "Монитор." + ViewModel.StaticVersion,
                type1 = "rubezh." + ViewModel.StaticVersion + "." + "monitor",
                model = "1.0"
            };

            var events = new List<Assad.modelInfoTypeEvent>();
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Изменено состояние монитора" });
            monitor.ModelInfo.@event = events.ToArray();

            var states = new List<Assad.modelInfoTypeState>();
            states.Add(CreateState("Тревога"));
            states.Add(CreateState("Внимание (предтревожное)"));
            states.Add(CreateState("Неисправность"));
            states.Add(CreateState("Требуется обслуживание"));
            states.Add(CreateState("Обход устройств"));
            states.Add(CreateState("Неопределено"));
            states.Add(CreateState("Норма(*)"));
            states.Add(CreateState("Норма"));

            monitor.ModelInfo.state = states.ToArray();

            return monitor;
        }

        static Assad.modelInfoTypeState CreateState(string name)
        {
            var state = new Assad.modelInfoTypeState();
            state.state = name;
            var stateValues = new List<Assad.modelInfoTypeStateValue>();
            stateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Есть" });
            stateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Нет" });
            state.value = stateValues.ToArray();
            return state;
        }
    }
}
