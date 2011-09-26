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

        public static Driver Convert(drvType innerDriver)
        {
            var driver = new Driver()
            {
                UID = new Guid(innerDriver.id),
                Name = innerDriver.name,
                ShortName = innerDriver.shortName,
                HasAddress = innerDriver.ar_no_addr != "1",
                IsAutoCreate = innerDriver.acr_enabled == "1",
                MinAutoCreateAddress = int.Parse(innerDriver.acr_from),
                MaxAutoCreateAddress = int.Parse(innerDriver.acr_to),
                HasAddressMask = innerDriver.addrMask != null
            };

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

            driver.IsDeviceOnShleif = innerDriver.addrMask != null && (innerDriver.addrMask == "[8(1)-15(2)];[0(1)-7(255)]" || innerDriver.addrMask == "[0(1)-8(30)]");

            driver.HasShleif = driver.ShleifCount == 0 ? false : true;

            if (innerDriver.name == "Насосная Станция")
                driver.UseParentAddressSystem = false;
            else
                driver.UseParentAddressSystem = innerDriver.options != null && innerDriver.options.Contains("UseParentAddressSystem");

            driver.IsChildAddressReservedRange = innerDriver.res_addr != null;

            driver.ChildAddressReserveRangeCount = driver.IsChildAddressReservedRange ? int.Parse(innerDriver.res_addr) : 0;
            

            if (innerDriver.addrMask == "[0(1)-8(8)]")
                driver.IsRangeEnabled = true;
            else
                driver.IsRangeEnabled = innerDriver.ar_enabled == "1";

            if (innerDriver.addrMask == "[0(1)-8(8)]")
                driver.MinAddress = 1;
            else
                driver.MinAddress = int.Parse(innerDriver.ar_from);

            if (innerDriver.addrMask == "[0(1)-8(8)]")
                driver.MaxAddress = 8;
            else
                driver.MaxAddress = int.Parse(innerDriver.ar_to);

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

            driver.ImageSource = imageSource;

            driver.HasImage = driver.ImageSource != @"Device_Device";
            driver.IsZoneDevice = ((innerDriver.minZoneCardinality == "0") && (innerDriver.maxZoneCardinality == "0")) == false;
            driver.IsIndicatorDevice = innerDriver.name == "Индикатор";
            driver.CanControl = driver.DriverType == DriverType.Valve;

            driver.IsBUtton = false;
            switch (driver.DriverType)
            {
                case DriverType.StopButton:
                case DriverType.StartButton:
                case DriverType.AutomaticButton:
                case DriverType.ShuzOffButton:
                case DriverType.ShuzOnButton:
                case DriverType.ShuzUnblockButton:
                    driver.IsBUtton = true;
                    break;
            }

            driver.Category = (DeviceCategoryType)int.Parse(innerDriver.cat);
            driver.CategoryName = EnumsConverter.CategoryTypeToCategoryName(driver.Category);

            driver.DeviceType = DeviceType.FireSecurity;
            if (innerDriver.options != null)
            {
                if (innerDriver.options.Contains("FireOnly"))
                {
                    driver.DeviceType = DeviceType.Fire;
                }

                if (innerDriver.options.Contains("SecOnly"))
                {
                    driver.DeviceType = DeviceType.Sequrity;
                }

                if (innerDriver.options.Contains("TechOnly"))
                {
                    driver.DeviceType = DeviceType.Technoligical;
                }
            }
            driver.DeviceTypeName = EnumsConverter.DeviceTypeToString(driver.DeviceType);

            driver.IsIgnore = (DriversHelper.DriverDataList.FirstOrDefault(x => (x.DriverId == innerDriver.id)).IgnoreLevel > 1);
            driver.IsAssadIgnore = (DriversHelper.DriverDataList.FirstOrDefault(x => (x.DriverId == innerDriver.id)).IgnoreLevel > 0);
            var driverData = DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverId == innerDriver.id && x.IgnoreLevel < 2);
            if (driverData != null)
            {
                //driver.DriverName = driverData.Name;
                driver.DriverType = driverData.DriverType;
            }

            var AllChildren = new List<drvType>();
            foreach (var childDriver in Metadata.drv)
            {
                var childClass = Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
                if (childClass.parent != null && childClass.parent.Any(x => x.clsid == innerDriver.clsid))
                {
                    if (childDriver.lim_parent != null && childDriver.lim_parent != innerDriver.id)
                        continue;

                    AllChildren.Add(childDriver);
                }
            }

            driver.Children = new List<Guid>(
                from drvType childInnerDriver in AllChildren
                where (DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverId == childInnerDriver.id).IgnoreLevel == 0)
                select new Guid(childInnerDriver.id));

            driver.AvaliableChildren = new List<Guid>(
                from drvType childInnerDriver in AllChildren
                where childInnerDriver.acr_enabled != "1"
                select new Guid(childInnerDriver.id));

            if (driver.DisableAutoCreateChildren)
                driver.AutoCreateChildren = new List<Guid>();
            else
            {
                driver.AutoCreateChildren = new List<Guid>(
                from drvType childInnerDriver in AllChildren
                where childInnerDriver.acr_enabled == "1"
                select new Guid(childInnerDriver.id));
            }

            driver.CanAddChildren = driver.AvaliableChildren.Count > 0;

            driver.Properties = new List<DriverProperty>();
            if (innerDriver.propInfo != null)
            {
                foreach (var internalProperty in innerDriver.propInfo)
                {
                    if (internalProperty.hidden == "1")
                        continue;
                    if (internalProperty.caption == "Заводской номер" || internalProperty.caption == "Версия микропрограммы")
                        continue;

                    var driverProperty = new DriverProperty();
                    driverProperty.Name = internalProperty.name;
                    driverProperty.Caption = internalProperty.caption;
                    driverProperty.Default = internalProperty.@default;
                    driverProperty.Visible = internalProperty.hidden == "0" && internalProperty.showOnlyInState == "0";
                    driverProperty.IsHidden = internalProperty.hidden == "1";

                    driverProperty.Parameters = new List<DriverPropertyParameter>();
                    if (internalProperty.param != null)
                    {
                        foreach (var firesecParameter in internalProperty.param)
                        {
                            driverProperty.Parameters.Add(new DriverPropertyParameter()
                            {
                                Name = firesecParameter.name,
                                Value = firesecParameter.value
                            });
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
                    driver.Parameters.Add(new Parameter()
                    {
                        Name = innerParameter.name,
                        Caption = innerParameter.caption,
                        Visible = innerParameter.hidden == "0" && innerParameter.showOnlyInState == "0"
                    });
                }
            }

            driver.States = new List<DriverState>();
            if (innerDriver.state != null)
            {
                foreach (var innerState in innerDriver.state)
                {
                    driver.States.Add(new DriverState()
                    {
                        Id = innerState.id,
                        Name = innerState.name,
                        AffectChildren = innerState.affectChildren == "1" ? true : false,
                        StateType = (StateType) int.Parse(innerState.@class),
                        IsManualReset = innerState.manualReset == "1" ? true : false,
                        CanResetOnPanel = innerState.CanResetOnPanel == "1" ? true : false,
                        IsAutomatic = innerState.type == "Auto" ? true : false,
                        Code = innerState.code
                    });
                }
            }

            if (innerDriver.options != null)
            {
                driver.DisableAutoCreateChildren = innerDriver.options.Contains("DisableAutoCreateChildren");
                driver.IsZoneLogicDevice = innerDriver.options.Contains("ExtendedZoneLogic");
                driver.CanDisable = innerDriver.options.Contains("Ignorable");
                driver.IsPlaceable = innerDriver.options.Contains("Placeable");
                driver.IsOutDevice = innerDriver.options.Contains("OutDevice");
                driver.IgnoreInZoneState = innerDriver.options.Contains("IgnoreInZoneState");

                driver.CanWriteDatabase = innerDriver.options.Contains("DeviceDatabaseWrite");
                driver.CanReadDatabase = innerDriver.options.Contains("DeviceDatabaseRead");
                driver.CanAutoDetect = innerDriver.options.Contains("CanAutoDetectInstances");
                driver.CanSynchonize = innerDriver.options.Contains("HasTimer");
                driver.CanReboot = innerDriver.options.Contains("RemoteReload");
                driver.CanGetDescription = innerDriver.options.Contains("DescriptionString");
                driver.CanSetPassword = innerDriver.options.Contains("PasswordManagement");
                driver.CanUpdateSoft = innerDriver.options.Contains("SoftUpdates");
                driver.CanExecuteCustomAdminFunctions = innerDriver.options.Contains("CustomIOCTLFunctions");
            }

            return driver;
        }
    }
}