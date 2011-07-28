using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using FiresecAPI.Models;

namespace DeviveModelManager
{
    public class TreeItem
    {
        public TreeItem()
        {
            Children = new ObservableCollection<TreeItem>();
        }

        public string Name { get; set; }
        public Assad.modelInfoType ModelInfo { get; set; }
        public ObservableCollection<TreeItem> Children { get; set; }
        Driver _driver;

        public void SetDriver(Driver driver)
        {
            _driver = driver;
            Name = _driver.Name;

            ModelInfo = new Assad.modelInfoType();
            ModelInfo.name = _driver.Name;
            ModelInfo.type1 = "rubezh." + ViewModel.StaticVersion + "." + _driver.Id;
            ModelInfo.model = "1.0";

            ModelInfo.@event = AddEvents().ToArray();
            ModelInfo.command = AddCommands().ToArray();
            ModelInfo.param = AddParameters().ToArray();
            ModelInfo.state = AddStates().ToArray();
        }

        List<Assad.modelInfoTypeEvent> AddEvents()
        {
            List<Assad.modelInfoTypeEvent> events = new List<Assad.modelInfoTypeEvent>();
            foreach (var state in CommonStatesHelper.States)
            {
                events.Add(new Assad.modelInfoTypeEvent() { @event = state });
            }
            return events;
        }


        List<Assad.modelInfoTypeCommand> AddCommands()
        {
            List<Assad.modelInfoTypeCommand> commands = new List<Assad.modelInfoTypeCommand>();
            foreach (var state in _driver.States)
            {
                if (state.IsManualReset)
                {
                    commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс " + state.Name });
                }
            }
            return commands;
        }


        List<Assad.modelInfoTypeParam> AddParameters()
        {
            List<Assad.modelInfoTypeParam> parameters = new List<Assad.modelInfoTypeParam>();
            if (_driver.HasAddress)
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
            foreach (var state in CommonStatesHelper.States)
            {
                StateValues.Add(new Assad.modelInfoTypeStateValue() { value = state });
            }
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
                if (paramInfo.Visible)
                {
                    States.Add(new Assad.modelInfoTypeState() { state = paramInfo.Caption });
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
