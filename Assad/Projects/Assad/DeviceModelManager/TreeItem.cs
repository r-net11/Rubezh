using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;

namespace DeviveModelManager
{
    public class TreeItem
    {
        public TreeItem()
        {
            Children = new ObservableCollection<TreeItem>();
        }
        public string Clsid { get; set; }
        public string DriverId { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public Assad.modelInfoType ModelInfo { get; set; }
        public ObservableCollection<TreeItem> Children { get; set; }
        public TreeItem Parent { get; set; }

        // этот метод формирует свойство ModelInfo на основе информации о драйвере устройства,
        // полученной из метаданных

        public void SetDriver(Firesec.Metadata.drvType driver)
        {
            List<Assad.modelInfoTypeEvent> AssadEvents = new List<Assad.modelInfoTypeEvent>();
            AssadEvents.Add(new Assad.modelInfoTypeEvent() { @event = "Валидация" });
            foreach (Firesec.Metadata.stateType comState in driver.state)
            {
                AssadEvents.Add(new Assad.modelInfoTypeEvent() { @event = comState.name });
                AssadEvents.Add(new Assad.modelInfoTypeEvent() { @event = "Сброс " + comState.name });
            }

            List<Assad.modelInfoTypeCommand> AssadCommands = new List<Assad.modelInfoTypeCommand>();
            if (PanelHelper.IsPanel(Name))
            {
                AssadCommands.Add(new Assad.modelInfoTypeCommand() { command = "Записать Конфигурацию" });
            }

            foreach (Firesec.Metadata.stateType comState in driver.state)
            {
                if (comState.manualReset == "1")
                {
                    AssadCommands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс " + comState.name });
                }
            }

            List<Assad.modelInfoTypeParam> AssadParams = new List<Assad.modelInfoTypeParam>();
            {
                AssadParams.Add(new Assad.modelInfoTypeParam() { param = "Примечание", type = "edit" });

                if (driver.ar_no_addr == "0")
                {
                    //Assad.modelInfoTypeParam addressParameter = new Assad.modelInfoTypeParam();
                    //addressParameter.param = "Адрес";
                    //addressParameter.type = "single";

                    //List<Assad.modelInfoTypeParamValue> addressParameterValues = new List<Assad.modelInfoTypeParamValue>();
                    //for (int i = AddressHelper.GetMinAddress(Name); i <= AddressHelper.GetMaxAddress(Name); i++)
                    //{
                    //    addressParameterValues.Add(new Assad.modelInfoTypeParamValue() { value = i.ToString() });
                    //}
                    //addressParameter.value = addressParameterValues.ToArray();
                    //AssadParams.Add(addressParameter);

                    AssadParams.Add(new Assad.modelInfoTypeParam() { param = "Адрес", type = "edit" });
                }

                if ((driver.minZoneCardinality == "1") && (driver.maxZoneCardinality == "1"))
                {
                    AssadParams.Add(new Assad.modelInfoTypeParam() { param = "Зона", type = "edit" });
                }
                if (driver.maxZoneCardinality == "-1")
                {
                    AssadParams.Add(new Assad.modelInfoTypeParam() { param = "Зоны", type = "edit" });
                }
            }
            int shleifCount = PanelHelper.GetShleifCount(ParentName);
            if (shleifCount > 0)
            {
                Assad.modelInfoTypeParam shleifParameter = new Assad.modelInfoTypeParam();
                shleifParameter.param = "Номер шлейфа (АЛС)";
                shleifParameter.type = "single";
                shleifParameter.value = new Assad.modelInfoTypeParamValue[shleifCount];
                for (int i = 0; i < shleifCount; i++)
                {
                    shleifParameter.value[i] = new Assad.modelInfoTypeParamValue();
                    shleifParameter.value[i].value = (i + 1).ToString();
                }
                AssadParams.Add(shleifParameter);
            }

            if (driver.propInfo != null)
            {
                foreach (Firesec.Metadata.propInfoType propInfo in driver.propInfo)
                {
                    Assad.modelInfoTypeParam customParam = new Assad.modelInfoTypeParam();
                    if (propInfo.hidden == "0")
                    {
                        if (!string.IsNullOrEmpty(propInfo.caption))
                        {
                            if (!string.IsNullOrEmpty(propInfo.type))
                            {
                                customParam.param = propInfo.caption;
                                if (propInfo.caption == "Адрес")
                                {
                                    customParam.param = "Адрес USB устройства в сети RS-485";
                                }
                                if ((propInfo.caption == "Заводской номер") || (propInfo.caption == "Версия микропрограммы"))
                                {
                                    continue;
                                }

                                if (propInfo.param != null)
                                {
                                    if (propInfo.param.Count() > 0)
                                    {
                                        customParam.type = "single";
                                        List<Assad.modelInfoTypeParamValue> customParamValues = new List<Assad.modelInfoTypeParamValue>();
                                        foreach (Firesec.Metadata.paramType paramType in propInfo.param)
                                        {
                                            Assad.modelInfoTypeParamValue modelInfoTypeParamValue = new Assad.modelInfoTypeParamValue();
                                            modelInfoTypeParamValue.value = paramType.name;
                                            customParamValues.Add(modelInfoTypeParamValue);
                                        }
                                        customParam.value = customParamValues.ToArray();
                                    }
                                }
                                else
                                {
                                    if ((propInfo.type == "pkBoolean") || (propInfo.type == "Bool"))
                                    {
                                        customParam.type = "single";
                                        customParam.value = new Assad.modelInfoTypeParamValue[2];
                                        customParam.value[0] = new Assad.modelInfoTypeParamValue();
                                        customParam.value[0].value = "Нет";
                                        customParam.value[1] = new Assad.modelInfoTypeParamValue();
                                        customParam.value[1].value = "Да";
                                    }
                                    else
                                    {
                                        customParam.type = "edit";
                                    }
                                }

                                AssadParams.Add(customParam);
                            }
                        }
                    }
                }
            }

            List<Assad.modelInfoTypeState> AssadStates = new List<Assad.modelInfoTypeState>();
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
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Нет состояния" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Отсутствует в конфигурации сервера оборудования" });
            StateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Нет связи с сервером оборудования" });
            AssadState.value = StateValues.ToArray();
            AssadStates.Add(AssadState);

            Assad.modelInfoTypeState AdditionalState = new Assad.modelInfoTypeState();
            AdditionalState.state = "Состояние дополнительно";
            List<Assad.modelInfoTypeStateValue> AdditionalStateValues = new List<Assad.modelInfoTypeStateValue>();
            foreach (Firesec.Metadata.stateType comState in driver.state)
            {
                AdditionalStateValues.Add(new Assad.modelInfoTypeStateValue() { value = comState.name });
            }
            AdditionalState.value = AdditionalStateValues.ToArray();
            AssadStates.Add(AdditionalState);

            Assad.modelInfoTypeState AssadConfigurationState = new Assad.modelInfoTypeState();
            AssadConfigurationState.state = "Конфигурация";
            List<Assad.modelInfoTypeStateValue> ConfigurationStateValues = new List<Assad.modelInfoTypeStateValue>();
            ConfigurationStateValues.Add(new Assad.modelInfoTypeStateValue() { value = "" });
            ConfigurationStateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Ошибка" });
            AssadConfigurationState.value = ConfigurationStateValues.ToArray();
            AssadStates.Add(AssadConfigurationState);

            // для теста
            AssadStates.Add(new Assad.modelInfoTypeState() { state = "Дополнительно" });

            ModelInfo = new Assad.modelInfoType();
            ModelInfo.name = Name;
            DriverId = driver.id;
            ModelInfo.type1 = "rubezh." + ViewModel.StaticVersion + "." + DriverId.ToString();
            ModelInfo.model = "1.0";

            if (AssadEvents.Count > 0)
                ModelInfo.@event = AssadEvents.ToArray();

            if (AssadCommands.Count > 0)
                ModelInfo.command = AssadCommands.ToArray();

            if (AssadParams.Count > 0)
                ModelInfo.param = AssadParams.ToArray();

            if (AssadStates.Count > 0)
                ModelInfo.state = AssadStates.ToArray();
        }

        // возвращается свойство ModelInfo в текстовом виде
        // исключительно для визуализации

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
