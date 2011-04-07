using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviveModelManager
{
    public class RootHelper
    {
        public static TreeItem CreateRoot()
        {
            TreeItem rootTreeItem = new TreeItem();
            rootTreeItem.ModelInfo = new Assad.modelInfoType();
            rootTreeItem.ModelInfo.name = "АПИ ОПС Рубеж." + ViewModel.StaticVersion;
            rootTreeItem.ModelInfo.type1 = "rubezh." + ViewModel.StaticVersion + "." + "F8340ECE-C950-498D-88CD-DCBABBC604F3";
            rootTreeItem.ModelInfo.model = "1.0";

            List<Assad.modelInfoTypeEvent> events = new List<Assad.modelInfoTypeEvent>();
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Выгрузка АПИ" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Загрузка АПИ" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Появление связи с сервером оборудования" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Пропадание связи с сервером оборудования" });
            rootTreeItem.ModelInfo.@event = events.ToArray();

            List<Assad.modelInfoTypeCommand> commands = new List<Assad.modelInfoTypeCommand>();
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Записать Конфигурацию" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Пожар" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Тревога" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Внимание" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Тест" });
            commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс Системная неисправность" });
            rootTreeItem.ModelInfo.command = commands.ToArray();

            List<Assad.modelInfoTypeParam> parameters = new List<Assad.modelInfoTypeParam>();
            parameters.Add(new Assad.modelInfoTypeParam() { param = "порт", type = "edit", @default="2002" });
            parameters.Add(new Assad.modelInfoTypeParam() { param = "IP адрес", type = "edit" });
            parameters.Add(new Assad.modelInfoTypeParam() { param = "Примечание", type = "edit" });
            Assad.modelInfoTypeParam modeParam = new Assad.modelInfoTypeParam() { param = "Режим", type = "single" };
            parameters.Add(modeParam);
            List<Assad.modelInfoTypeParamValue> modeValues = new List<Assad.modelInfoTypeParamValue>();
            modeValues.Add(new Assad.modelInfoTypeParamValue(){value="Мониторинг"});
            modeValues.Add(new Assad.modelInfoTypeParamValue(){value="Конфигурирование"});
            modeParam.value = modeValues.ToArray();
            rootTreeItem.ModelInfo.param = parameters.ToArray();

            rootTreeItem.ModelInfo.state = new Assad.modelInfoTypeState[1];

            rootTreeItem.ModelInfo.state[0] = new Assad.modelInfoTypeState();
            rootTreeItem.ModelInfo.state[0] = new Assad.modelInfoTypeState();
            rootTreeItem.ModelInfo.state[0].state = "Состояние";
            List<Assad.modelInfoTypeStateValue> StateValues = new List<Assad.modelInfoTypeStateValue>();
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Тревога" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Внимание (предтревожное)" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Неисправность" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Требуется обслуживание" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Обход устройств" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Неопределено" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Норма(*)" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Норма" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Отсутствует в конфигурации сервера оборудования" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Нет связи с сервером оборудования" });
            rootTreeItem.ModelInfo.state[0].value = StateValues.ToArray();

            return rootTreeItem;
        }
    }
}
