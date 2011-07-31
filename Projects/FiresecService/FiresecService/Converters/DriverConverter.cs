using System;
using System.Collections.Generic;
using System.Linq;
using Firesec.Metadata;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class DriverConverter
    {
        public static Firesec.Metadata.config Metadata;

        public static Driver Convert(configDrv innerDriver)
        {
            Driver driver = new Driver();
            driver.Id = innerDriver.id;
            driver.Name = innerDriver.name;
            driver.ShortName = innerDriver.shortName; ;
            driver.HasAddress = innerDriver.ar_no_addr != "1";

            driver.CanEditAddress = true;
            if (innerDriver.ar_no_addr != null)
            {
                if (innerDriver.ar_no_addr == "1")
                    driver.CanEditAddress = false;

                if (innerDriver.acr_enabled == "1")
                    driver.CanEditAddress = false;
            }

            driver.AddressMask = innerDriver.addrMask;
            driver.ChildAddressMask = innerDriver.childAddrMask;

            driver.ShleifCount = 0;
            if (innerDriver.childAddrMask != null)
            {
                switch (innerDriver.childAddrMask)
                {
                    case "[8(1)-15(2)];[0(1)-7(255)]":
                        driver.ShleifCount = 2;
                        break;

                    case "[8(1)-15(4)];[0(1)-7(255)]":
                        driver.ShleifCount = 4;
                        break;

                    case "[8(1)-15(10)];[0(1)-7(255)]":
                        driver.ShleifCount = 10;
                        break;
                }
            }

            driver.IsDeviceOnShleif = ((innerDriver.addrMask != null) && ((innerDriver.addrMask == "[8(1)-15(2)];[0(1)-7(255)]") || (innerDriver.addrMask == "[0(1)-8(30)]")));

            driver.HasShleif = driver.ShleifCount == 0 ? false : true;

            if (innerDriver.name == "Насосная Станция")
                driver.UseParentAddressSystem = false;
            else
                driver.UseParentAddressSystem = ((innerDriver.options != null) && (innerDriver.options.Contains("UseParentAddressSystem")));

            driver.IsChildAddressReservedRange = (innerDriver.res_addr != null);

            driver.ChildAddressReserveRangeCount = driver.IsChildAddressReservedRange ? System.Convert.ToInt32(innerDriver.res_addr) : 0;
            driver.DisableAutoCreateChildren = (innerDriver.options != null) && (innerDriver.options.Contains("DisableAutoCreateChildren"));

            if (innerDriver.addrMask == "[0(1)-8(8)]")
                driver.IsRangeEnabled = true;
            else
                driver.IsRangeEnabled = innerDriver.ar_enabled == "1";

            if (innerDriver.addrMask == "[0(1)-8(8)]")
                driver.MinAddress = 1;
            else
                driver.MinAddress = System.Convert.ToInt32(innerDriver.ar_from);

            if (innerDriver.addrMask == "[0(1)-8(8)]")
                driver.MaxAddress = 8;
            else
                driver.MaxAddress = System.Convert.ToInt32(innerDriver.ar_to);

            driver.IsAutoCreate = innerDriver.acr_enabled == "1";
            driver.MinAutoCreateAddress = System.Convert.ToInt32(innerDriver.acr_from);
            driver.MaxAutoCreateAddress = System.Convert.ToInt32(innerDriver.acr_to);
            driver.HasAddressMask = innerDriver.addrMask != null;

            string imageSource;
            if (string.IsNullOrEmpty(innerDriver.dev_icon) == false)
            {
                imageSource = innerDriver.dev_icon;
            }
            else
            {
                var metadataClass = Metadata.@class.FirstOrDefault(x => x.clsid == innerDriver.clsid);
                imageSource = metadataClass.param.FirstOrDefault(x => x.name == "Icon").value;
            }

            driver.ImageSource = @"C:/Program Files/Firesec/Icons/" + imageSource + ".ico";

            driver.HasImage = driver.ImageSource != @"C:/Program Files/Firesec/Icons/Device_Device.ico";
            driver.IsZoneDevice = !((innerDriver.minZoneCardinality == "0") && (innerDriver.maxZoneCardinality == "0"));
            driver.IsZoneLogicDevice = ((innerDriver.options != null) && (innerDriver.options.Contains("ExtendedZoneLogic")));
            driver.CanDisable = (innerDriver.options != null) && (innerDriver.options.Contains("Ignorable"));
            driver.IsPlaceable = (innerDriver.options != null) && (innerDriver.options.Contains("Placeable"));
            driver.IsIndicatorDevice = (innerDriver.name == "Индикатор");
            driver.CanControl = (driver.DriverName == "Задвижка");

            driver.IsBUtton = false;
            switch (driver.DriverName)
            {
                case "Кнопка останова СПТ":
                case "Кнопка запуска СПТ":
                case "Кнопка управления автоматикой":
                case "Кнопка вкл автоматики ШУЗ и насосов в направлении":
                case "Кнопка выкл автоматики ШУЗ и насосов в направлении":
                case "Кнопка разблокировки автоматики ШУЗ в направлении":
                    driver.IsBUtton = true;
                    break;
            }

            driver.IsOutDevice = (innerDriver.options != null) && (innerDriver.options.Contains("OutDevice"));

            switch (innerDriver.cat)
            {
                case "0":
                    driver.Category = DeviceCategoryType.Other;
                    driver.CategoryName = "Прочие устройства";
                    break;

                case "1":
                    driver.Category = DeviceCategoryType.Device;
                    driver.CategoryName = "Приборы";
                    break;

                case "2":
                    driver.Category = DeviceCategoryType.Sensor;
                    driver.CategoryName = "Датчики";
                    break;

                case "3":
                    driver.Category = DeviceCategoryType.Effector;
                    driver.CategoryName = "ИУ";
                    break;

                case "4":
                    driver.Category = DeviceCategoryType.Communication;
                    driver.CategoryName = "Сеть передачи данных";
                    break;

                case "5":
                    driver.Category = DeviceCategoryType.None;
                    driver.CategoryName = "Не указано";
                    break;

                case "6":
                    driver.Category = DeviceCategoryType.RemoteServer;
                    driver.CategoryName = "Удаленный сервер";
                    break;

                default:
                    driver.Category = DeviceCategoryType.None;
                    driver.CategoryName = "Не указано";
                    break;
            }

            driver.DeviceType = DeviceType.FireSecurity;
            driver.DeviceTypeName = "охранно-пожарный";

            if (innerDriver.options != null)
            {
                if (innerDriver.options.Contains("FireOnly"))
                {
                    driver.DeviceType = DeviceType.Fire;
                    driver.DeviceTypeName = "пожарный";
                }

                if (innerDriver.options.Contains("SecOnly"))
                {
                    driver.DeviceType = DeviceType.Sequrity;
                    driver.DeviceTypeName = "охранный";
                }

                if (innerDriver.options.Contains("TechOnly"))
                {
                    driver.DeviceType = DeviceType.Technoligical;
                    driver.DeviceTypeName = "технологический";
                }
            }

            driver.IsIgnore = (DriversHelper.DriverDataList.FirstOrDefault(x => (x.DriverId == innerDriver.id)).IgnoreLevel > 1);
            driver.IsAssadIgnore = (DriversHelper.DriverDataList.FirstOrDefault(x => (x.DriverId == innerDriver.id)).IgnoreLevel > 0);
            var driverData = DriversHelper.DriverDataList.FirstOrDefault(x => (x.DriverId == innerDriver.id) && (x.IgnoreLevel < 2));
            if (driverData != null)
            {
                driver.DriverName = driverData.Name;
            }

            List<configDrv> AllChildren = new List<configDrv>();
            foreach (var childDriver in Metadata.drv)
            {
                var childClass = Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
                if ((childClass.parent != null) && (childClass.parent.Any(x => x.clsid == innerDriver.clsid)))
                {
                    if ((childDriver.lim_parent != null) && (childDriver.lim_parent != innerDriver.id))
                        continue;

                    AllChildren.Add(childDriver);
                }
            }

            driver.Children = new List<string>(
                from configDrv childInnerDriver in AllChildren
                where (DriversHelper.DriverDataList.FirstOrDefault(x => (x.DriverId == childInnerDriver.id)).IgnoreLevel == 0)
                select childInnerDriver.id);

            driver.AvaliableChildren = new List<string>(
                from configDrv childInnerDriver in AllChildren
                where childInnerDriver.acr_enabled != "1"
                select childInnerDriver.id);

            if (driver.DisableAutoCreateChildren)
                driver.AutoCreateChildren = new List<string>();
            else
            {
                driver.AutoCreateChildren = new List<string>(
                from configDrv childInnerDriver in AllChildren
                where childInnerDriver.acr_enabled == "1"
                select childInnerDriver.id);
            }

            driver.CanAddChildren = (driver.AvaliableChildren.Count > 0);

            driver.Properties = new List<DriverProperty>();
            if (innerDriver.propInfo != null)
            {
                foreach (var internalProperty in innerDriver.propInfo)
                {
                    if (internalProperty.hidden == "1")
                        continue;
                    if ((internalProperty.caption == "Заводской номер") || (internalProperty.caption == "Версия микропрограммы"))
                        continue;

                    DriverProperty driverProperty = new DriverProperty();
                    driverProperty.Name = internalProperty.name;
                    driverProperty.Caption = internalProperty.caption;
                    driverProperty.Default = internalProperty.@default;
                    driverProperty.Visible = ((internalProperty.hidden == "0") && (internalProperty.showOnlyInState == "0"));
                    driverProperty.IsHidden = (internalProperty.hidden == "1");

                    driverProperty.Parameters = new List<DriverPropertyParameter>();
                    if (internalProperty.param != null)
                    {
                        foreach (var firesecParameter in internalProperty.param)
                        {
                            DriverPropertyParameter driverPropertyParameter = new DriverPropertyParameter();
                            driverPropertyParameter.Name = firesecParameter.name;
                            driverPropertyParameter.Value = firesecParameter.value;
                            driverProperty.Parameters.Add(driverPropertyParameter);
                        }
                    }

                    if (internalProperty.param != null)
                    {
                        driverProperty.DriverPropertyType = DriverPropertyTypeEnum.EnumType;
                    }
                    else
                    {
                        switch (internalProperty.type)
                        {
                            case "String":
                                driverProperty.DriverPropertyType = DriverPropertyTypeEnum.StringType;
                                break;

                            case "Int":
                            case "Double":
                                driverProperty.DriverPropertyType = DriverPropertyTypeEnum.IntType;
                                break;

                            case "Byte":
                                driverProperty.DriverPropertyType = DriverPropertyTypeEnum.ByteType;
                                break;

                            case "Bool":
                                driverProperty.DriverPropertyType = DriverPropertyTypeEnum.BoolType;
                                break;

                            default:
                                throw new Exception("Неизвестный тип свойства");
                        }
                    }
                    driver.Properties.Add(driverProperty);
                }
            }

            driver.Parameters = new List<Parameter>();
            if (innerDriver.paramInfo != null)
            {
                foreach (var innerParameter in innerDriver.paramInfo)
                {
                    Parameter parameter = new Parameter();
                    parameter.Name = innerParameter.name;
                    parameter.Caption = innerParameter.caption;
                    parameter.Visible = ((innerParameter.hidden == "0") && (innerParameter.showOnlyInState == "0"));
                    driver.Parameters.Add(parameter);
                }
            }

            driver.States = new List<InnerState>();
            if (innerDriver.state != null)
            {
                foreach (var innerState in innerDriver.state)
                {
                    InnerState state = new InnerState();
                    state.Id = innerState.id;
                    state.Name = innerState.name;
                    state.AffectChildren = innerState.affectChildren == "1" ? true : false;
                    state.Priority = System.Convert.ToInt32(innerState.@class);
                    state.IsManualReset = innerState.manualReset == "1" ? true : false;
                    state.CanResetOnPanel = innerState.CanResetOnPanel == "1" ? true : false;
                    state.IsAutomatic = innerState.type == "Auto" ? true : false;
                    state.Code = innerState.code;
                    driver.States.Add(state);
                }
            }

            return driver;
        }
    }
}
