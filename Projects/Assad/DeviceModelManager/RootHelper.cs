using System.Collections.Generic;

namespace DeviveModelManager
{
    public class RootHelper
    {
        public static TreeItem CreateRoot()
        {
            var rootTreeItem = new TreeItem();
            rootTreeItem.ModelInfo = new Assad.modelInfoType()
            {
                name = "АПИ ОПС Рубеж." + ViewModel.StaticVersion,
                type1 = "rubezh." + ViewModel.StaticVersion + "." + "F8340ECE-C950-498D-88CD-DCBABBC604F3",
                model = "1.0"
            };

            var events = new List<Assad.modelInfoTypeEvent>();
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Выгрузка АПИ" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Загрузка АПИ" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Появление связи с сервером оборудования" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Пропадание связи с сервером оборудования" });
            rootTreeItem.ModelInfo.@event = events.ToArray();

            var commands = new List<Assad.modelInfoTypeCommand>();
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Записать Конфигурацию" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Пожар" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Тревога" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Внимание" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Тест" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Системная неисправность" });
            rootTreeItem.ModelInfo.command = commands.ToArray();

            var parameters = new List<Assad.modelInfoTypeParam>();
            parameters.Add(new Assad.modelInfoTypeParam() { param = "порт", type = "edit", @default="2002" });
            parameters.Add(new Assad.modelInfoTypeParam() { param = "IP адрес", type = "edit" });
            parameters.Add(new Assad.modelInfoTypeParam() { param = "Примечание", type = "edit" });
            var modeParam = new Assad.modelInfoTypeParam() { param = "Режим", type = "single" };
            parameters.Add(modeParam);
            var modeValues = new List<Assad.modelInfoTypeParamValue>();
            modeValues.Add(new Assad.modelInfoTypeParamValue(){value="Мониторинг"});
            modeValues.Add(new Assad.modelInfoTypeParamValue(){value="Конфигурирование"});
            modeParam.value = modeValues.ToArray();
            rootTreeItem.ModelInfo.param = parameters.ToArray();

            rootTreeItem.ModelInfo.state = new Assad.modelInfoTypeState[1];

            rootTreeItem.ModelInfo.state[0] = new Assad.modelInfoTypeState();
            rootTreeItem.ModelInfo.state[0].state = "Состояние";
            var StateValues = new List<Assad.modelInfoTypeStateValue>();
            foreach (var state in CommonStatesHelper.States)
            {
                StateValues.Add(new Assad.modelInfoTypeStateValue() { value = state });
            }
            rootTreeItem.ModelInfo.state[0].value = StateValues.ToArray();

            return rootTreeItem;
        }
    }
}
