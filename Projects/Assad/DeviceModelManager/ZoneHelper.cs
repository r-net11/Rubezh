using System.Collections.Generic;

namespace DeviveModelManager
{
    public static class ZoneHelper
    {
        public static TreeItem CreateZone()
        {
            var zoneTreeItem = new TreeItem();
            zoneTreeItem.Name = "Zone";
            zoneTreeItem.ModelInfo = new Assad.modelInfoType()
            {
                type1 = "rubezh." + ViewModel.StaticVersion + "." + "zone",
                model = "1.0",
                name = "Зона"
            };

            var events = new List<Assad.modelInfoTypeEvent>();
            foreach (var state in CommonStatesHelper.States)
            {
                events.Add(new Assad.modelInfoTypeEvent() { @event = state });
            }
            zoneTreeItem.ModelInfo.@event = events.ToArray();

            var commands = new List<Assad.modelInfoTypeCommand>();
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Функция с зоной" });
            zoneTreeItem.ModelInfo.command = commands.ToArray();

            var parameters = new List<Assad.modelInfoTypeParam>();
            parameters.Add(new Assad.modelInfoTypeParam() { param = "Номер зоны", type = "edit" });
            zoneTreeItem.ModelInfo.param = parameters.ToArray();

            zoneTreeItem.ModelInfo.state = new Assad.modelInfoTypeState[6];
            zoneTreeItem.ModelInfo.state[0] = new Assad.modelInfoTypeState();
            zoneTreeItem.ModelInfo.state[0].state = "Состояние";
            var StateValues = new List<Assad.modelInfoTypeStateValue>();
            foreach (var state in CommonStatesHelper.States)
            {
                StateValues.Add(new Assad.modelInfoTypeStateValue() { value = state });
            }
            zoneTreeItem.ModelInfo.state[0].value = StateValues.ToArray();
            zoneTreeItem.ModelInfo.state[1] = new Assad.modelInfoTypeState()
            {
                state = "Наименование"
            };
            zoneTreeItem.ModelInfo.state[2] = new Assad.modelInfoTypeState();
            zoneTreeItem.ModelInfo.state[3] = new Assad.modelInfoTypeState()
            {
                state = "Время эвакуации"
            };
            zoneTreeItem.ModelInfo.state[4] = new Assad.modelInfoTypeState()
            {
                state = "Примечание"
            };
            zoneTreeItem.ModelInfo.state[5] = new Assad.modelInfoTypeState()
            {
                state = "Назначение зоны"
            };
            return zoneTreeItem;
        }
    }
}
