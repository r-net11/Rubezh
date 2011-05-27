using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

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
        Firesec.Metadata.drvType Driver { get; set; }

        // этот метод формирует свойство ModelInfo на основе информации о драйвере устройства,
        // полученной из метаданных

        public void SetDriver(Firesec.Metadata.drvType driver)
        {
            this.Driver = driver;
            Name = driver.name;

            ModelInfo = new Assad.modelInfoType();
            ModelInfo.name = driver.name;
            DriverId = driver.id;
            ModelInfo.type1 = "rubezh." + ViewModel.StaticVersion + "." + DriverId.ToString();
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
            foreach (Firesec.Metadata.stateType comState in Driver.state)
            {
                if (comState.manualReset == "1")
                {
                    commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс " + comState.name });
                }
            }
            return commands;
        }


        List<Assad.modelInfoTypeParam> AddParameters()
        {
            List<Assad.modelInfoTypeParam> parameters = new List<Assad.modelInfoTypeParam>();
//            parameters.Add(new Assad.modelInfoTypeParam() { param = "Примечание", type = "edit" });

            if (Driver.ar_no_addr == "0")
            {
                parameters.Add(new Assad.modelInfoTypeParam() { param = "Адрес", type = "edit" });
            }

            //if ((Driver.minZoneCardinality == "1") && (Driver.maxZoneCardinality == "1"))
            //{
            //    parameters.Add(new Assad.modelInfoTypeParam() { param = "Зона", type = "edit" });
            //}
            //else
            //{
            //    if ((Driver.options != null) && (Driver.options.Contains("ExtendedZoneLogic")))
            //    {
            //        parameters.Add(new Assad.modelInfoTypeParam() { param = "Настройка включения по состоянию зон", type = "edit" });
            //    }
            //}

            //if (Driver.propInfo != null)
            //{
            //    foreach (Firesec.Metadata.propInfoType propInfo in Driver.propInfo)
            //    {
            //        Assad.modelInfoTypeParam customParam = new Assad.modelInfoTypeParam();
            //        if (propInfo.hidden == "0")
            //        {
            //            if (!string.IsNullOrEmpty(propInfo.caption))
            //            {
            //                if (!string.IsNullOrEmpty(propInfo.type))
            //                {
            //                    customParam.param = propInfo.caption;
            //                    if (propInfo.caption == "Адрес")
            //                    {
            //                        customParam.param = "Адрес USB устройства в сети RS-485";
            //                    }
            //                    if ((propInfo.caption == "Заводской номер") || (propInfo.caption == "Версия микропрограммы"))
            //                    {
            //                        continue;
            //                    }

            //                    if (propInfo.param != null)
            //                    {
            //                        if (propInfo.param.Count() > 0)
            //                        {
            //                            customParam.type = "single";
            //                            List<Assad.modelInfoTypeParamValue> customParamValues = new List<Assad.modelInfoTypeParamValue>();
            //                            foreach (Firesec.Metadata.paramType paramType in propInfo.param)
            //                            {
            //                                Assad.modelInfoTypeParamValue modelInfoTypeParamValue = new Assad.modelInfoTypeParamValue();
            //                                modelInfoTypeParamValue.value = paramType.name;
            //                                customParamValues.Add(modelInfoTypeParamValue);
            //                            }
            //                            customParam.value = customParamValues.ToArray();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if ((propInfo.type == "pkBoolean") || (propInfo.type == "Bool"))
            //                        {
            //                            customParam.type = "checkbox";
            //                        }
            //                        else
            //                        {
            //                            customParam.type = "edit";
            //                        }
            //                    }

            //                    parameters.Add(customParam);
            //                }
            //            }
            //        }
            //    }
            //}
            return parameters;
        }

        List<Assad.modelInfoTypeState> AddStates()
        {
// данные для вставки новых состояний ->>>

            //if (Driver.propInfo != null)
            //{
            //    foreach (Firesec.Metadata.propInfoType propInfo in Driver.propInfo)
            //    {
            //        Assad.modelInfoTypeParam customParam = new Assad.modelInfoTypeParam();
            //        if (propInfo.hidden == "0")
            //        {
            //            if (!string.IsNullOrEmpty(propInfo.caption))
            //            {
            //                if (!string.IsNullOrEmpty(propInfo.type))
            //                {
            //                    customParam.param = propInfo.caption;
            //                    if (propInfo.caption == "Адрес")
            //                    {
            //                        customParam.param = "Адрес USB устройства в сети RS-485";
            //                    }
            //                    if ((propInfo.caption == "Заводской номер") || (propInfo.caption == "Версия микропрограммы"))
            //                    {
            //                        continue;
            //                    }

            //                    if (propInfo.param != null)
            //                    {
            //                        if (propInfo.param.Count() > 0)
            //                        {
            //                            customParam.type = "single";
            //                            List<Assad.modelInfoTypeParamValue> customParamValues = new List<Assad.modelInfoTypeParamValue>();
            //                            foreach (Firesec.Metadata.paramType paramType in propInfo.param)
            //                            {
            //                                Assad.modelInfoTypeParamValue modelInfoTypeParamValue = new Assad.modelInfoTypeParamValue();
            //                                modelInfoTypeParamValue.value = paramType.name;
            //                                customParamValues.Add(modelInfoTypeParamValue);
            //                            }
            //                            customParam.value = customParamValues.ToArray();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if ((propInfo.type == "pkBoolean") || (propInfo.type == "Bool"))
            //                        {
            //                            customParam.type = "checkbox";
            //                        }
            //                        else
            //                        {
            //                            customParam.type = "edit";
            //                        }
            //                    }

            //                    parameters.Add(customParam);
            //                }
            //            }
            //        }
            //    }
            //}
            
            
//<<--            
            
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
            //            parameters.Add(new Assad.modelInfoTypeParam() { param = "Примечание", type = "edit" });
            States.Add(new Assad.modelInfoTypeState() { state = "Примечание" });
            //if ((Driver.minZoneCardinality == "1") && (Driver.maxZoneCardinality == "1"))
            //{
            //    parameters.Add(new Assad.modelInfoTypeParam() { param = "Зона", type = "edit" });
            //}
            //else
            //{
            //    if ((Driver.options != null) && (Driver.options.Contains("ExtendedZoneLogic")))
            //    {
            //        parameters.Add(new Assad.modelInfoTypeParam() { param = "Настройка включения по состоянию зон", type = "edit" });
            //    }
            //}

            if ((Driver.minZoneCardinality == "1") && (Driver.maxZoneCardinality == "1"))
            {
                States.Add(new Assad.modelInfoTypeState() { state = "Зона"});
            }
            else
            {
                if ((Driver.options != null) && (Driver.options.Contains("ExtendedZoneLogic")))
                {
                    States.Add(new Assad.modelInfoTypeState() { state = "Настройка включения по состоянию зон"});
                }
            }

            //if (Driver.propInfo != null)
            //{
            //    foreach (Firesec.Metadata.propInfoType propInfo in Driver.propInfo)
            //    {
            //        Assad.modelInfoTypeParam customParam = new Assad.modelInfoTypeParam();
            //        if (propInfo.hidden == "0")
            //        {
            //            if (!string.IsNullOrEmpty(propInfo.caption))
            //            {
            //                if (!string.IsNullOrEmpty(propInfo.type))
            //                {
            //                    customParam.param = propInfo.caption;
            //                    if (propInfo.caption == "Адрес")
            //                    {
            //                        customParam.param = "Адрес USB устройства в сети RS-485";
            //                    }
            //                    if ((propInfo.caption == "Заводской номер") || (propInfo.caption == "Версия микропрограммы"))
            //                    {
            //                        continue;
            //                    }

            //                    if (propInfo.param != null)
            //                    {
            //                        if (propInfo.param.Count() > 0)
            //                        {
            //                            customParam.type = "single";
            //                            List<Assad.modelInfoTypeParamValue> customParamValues = new List<Assad.modelInfoTypeParamValue>();
            //                            foreach (Firesec.Metadata.paramType paramType in propInfo.param)
            //                            {
            //                                Assad.modelInfoTypeParamValue modelInfoTypeParamValue = new Assad.modelInfoTypeParamValue();
            //                                modelInfoTypeParamValue.value = paramType.name;
            //                                customParamValues.Add(modelInfoTypeParamValue);
            //                            }
            //                            customParam.value = customParamValues.ToArray();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if ((propInfo.type == "pkBoolean") || (propInfo.type == "Bool"))
            //                        {
            //                            customParam.type = "checkbox";
            //                        }
            //                        else
            //                        {
            //                            customParam.type = "edit";
            //                        }
            //                    }

            //                    parameters.Add(customParam);
            //                }
            //            }
            //        }
            //    }
            //}
//***
            if (Driver.propInfo != null)
            {
                foreach (Firesec.Metadata.propInfoType propInfo in Driver.propInfo)
                {
                    Assad.modelInfoTypeState customParam = new Assad.modelInfoTypeState();
                    if (propInfo.hidden == "0")
                    {
                        if (!string.IsNullOrEmpty(propInfo.caption))
                        {
                            if (!string.IsNullOrEmpty(propInfo.type))
                            {
                                customParam.state = propInfo.caption;
                                if (propInfo.caption == "Адрес")
                                {
                                    customParam.state = "Адрес USB устройства в сети RS-485";
                                }
                                if ((propInfo.caption == "Заводской номер") || (propInfo.caption == "Версия микропрограммы"))
                                {
                                    continue;
                                }

                                //if (propInfo.param != null)
                                //{
                                //    if (propInfo.param.Count() > 0)
                                //    {
                                //        customParam.type = "single";
                                //        List<Assad.modelInfoTypeParamValue> customParamValues = new List<Assad.modelInfoTypeParamValue>();
                                //        foreach (Firesec.Metadata.paramType paramType in propInfo.param)
                                //        {
                                //            Assad.modelInfoTypeParamValue modelInfoTypeParamValue = new Assad.modelInfoTypeParamValue();
                                //            modelInfoTypeParamValue.value = paramType.name;
                                //            customParamValues.Add(modelInfoTypeParamValue);
                                //        }
                                //        customParam.value = customParamValues.ToArray();
                                //    }
                                //}
                                //else
                                //{
                                //    if ((propInfo.type == "pkBoolean") || (propInfo.type == "Bool"))
                                //    {
                                //        customParam.type = "checkbox";
                                //    }
                                //    else
                                //    {
                                //        customParam.type = "edit";
                                //    }
                                //}

                                States.Add(customParam);
                            }
                        }
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
            if (Driver.paramInfo != null)
            {
                foreach (Firesec.Metadata.paramInfoType paramInfo in Driver.paramInfo)
                {
                    if ((paramInfo.hidden == "0") && (paramInfo.showOnlyInState == "0"))
                    {
                        States.Add(new Assad.modelInfoTypeState() { state = paramInfo.caption });
                    }
                }
            }

            return States;
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
