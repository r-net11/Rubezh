using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using FiresecClient.Models;

namespace DeviveModelManager
{
    public class TreeItem
    {
        public TreeItem()
        {
            Children = new ObservableCollection<TreeItem>();
        }

        public string DriverId { get; set; }
        public string Name { get; set; }
        public Assad.modelInfoType ModelInfo { get; set; }
        public ObservableCollection<TreeItem> Children { get; set; }
        public TreeItem Parent { get; set; }
        Driver _driver;

        public void SetDriver(Driver driver)
        {
            _driver = driver;
            Name = _driver.Name;

            ModelInfo = new Assad.modelInfoType();
            ModelInfo.name = _driver.Name;
            DriverId = _driver.Id;
            ModelInfo.type1 = "rubezh." + ViewModel.StaticVersion + "." + DriverId;
            ModelInfo.model = "1.0";

            ModelInfo.@event = AddEvents().ToArray();
            ModelInfo.command = AddCommands().ToArray();
            ModelInfo.param = AddParameters().ToArray();
            ModelInfo.state = AddStates().ToArray();
        }

        List<Assad.modelInfoTypeEvent> AddEvents()
        {
            List<Assad.modelInfoTypeEvent> events = new List<Assad.modelInfoTypeEvent>();
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Тревога" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Внимание (предтревожное)" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Неисправность" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Требуется обслуживание" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Обход устройств" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Неопределено" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Норма(*)" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Норма" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Отсутствует в конфигурации сервера оборудования" });
            events.Add(new Assad.modelInfoTypeEvent() { @event = "Нет связи с сервером оборудования" });
            return events;
        }


        List<Assad.modelInfoTypeCommand> AddCommands()
        {
            List<Assad.modelInfoTypeCommand> commands = new List<Assad.modelInfoTypeCommand>();
            foreach (var state in _driver.States)
            {
                if (state.manualReset == "1")
                {
                    commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс " + state.name });
                }
            }
            return commands;
        }


        List<Assad.modelInfoTypeParam> AddParameters()
        {
            List<Assad.modelInfoTypeParam> parameters = new List<Assad.modelInfoTypeParam>();
            if (_driver.HasAddress == false)
            {
                parameters.Add(new Assad.modelInfoTypeParam() { param = "Адрес", type = "edit" });
            }
            return parameters;
        }

        List<Assad.modelInfoTypeState> AddStates()
        {
            List<Assad.modelInfoTypeState> States = new List<Assad.modelInfoTypeState>();
            Assad.modelInfoTypeState AssadState = new Assad.modelInfoTypeState();
            AssadState.state = "Состояние";
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
            AssadState.value = StateValues.ToArray();
            States.Add(AssadState);

            States.Add(new Assad.modelInfoTypeState() { state = "Примечание" });

            if (_driver.IsZoneDevice)
            {
                States.Add(new Assad.modelInfoTypeState() { state = "Зона" });
            }
            if (_driver.IsZoneLogicDevice)
            {
                States.Add(new Assad.modelInfoTypeState() { state = "Настройка включения по состоянию зон" });
            }

            foreach (var propInfo in _driver.Properties)
            {
                Assad.modelInfoTypeState customParam = new Assad.modelInfoTypeState();
                if (propInfo.IsHidden == false)
                {
                    if (!string.IsNullOrEmpty(propInfo.Caption))
                    {
                        customParam.state = propInfo.Caption;
                        if (propInfo.Caption == "Адрес")
                        {
                            customParam.state = "Адрес USB устройства в сети RS-485";
                        }
                        States.Add(customParam);
                    }
                }
            }

            Assad.modelInfoTypeState AssadConfigurationState = new Assad.modelInfoTypeState();
            AssadConfigurationState.state = "Конфигурация";
            List<Assad.modelInfoTypeStateValue> ConfigurationStateValues = new List<Assad.modelInfoTypeStateValue>();
            ConfigurationStateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Норма" });
            ConfigurationStateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Ошибка" });
            AssadConfigurationState.value = ConfigurationStateValues.ToArray();
            States.Add(AssadConfigurationState);
            foreach (var paramInfo in _driver.Parameters)
            {
                if ((paramInfo.hidden == "0") && (paramInfo.showOnlyInState == "0"))
                {
                    States.Add(new Assad.modelInfoTypeState() { state = paramInfo.caption });
                }
            }

            return States;
        }

        public string ModelInfoXml
        {
            get
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Assad.modelInfoType));
                MemoryStream memoryStream = new MemoryStream();
                serializer.Serialize(memoryStream, ModelInfo);
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
