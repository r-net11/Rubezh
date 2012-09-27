using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.Models.Metadata;
using FiresecAPI;
using FiresecAPI.Models;

namespace Firesec
{
	public static class DriverConverter
	{
		public static Firesec.Models.Metadata.config Metadata;

		public static Driver Convert(drvType innerDriver)
		{
			var driver = new Driver()
			{
				UID = new Guid(innerDriver.id),
				StringUID = innerDriver.id,
				Name = innerDriver.name,
				ShortName = innerDriver.shortName,
				HasAddress = innerDriver.ar_no_addr != "1",
				IsAutoCreate = innerDriver.acr_enabled == "1",
				MinAutoCreateAddress = int.Parse(innerDriver.acr_from),
				MaxAutoCreateAddress = int.Parse(innerDriver.acr_to),
				HasAddressMask = innerDriver.addrMask != null,
				IsAlternativeUSB = innerDriver.altIntf != null,
				AddressMask = innerDriver.addrMask,
				ChildAddressMask = innerDriver.childAddrMask,
				IsZoneDevice = ((innerDriver.minZoneCardinality == "0") && (innerDriver.maxZoneCardinality == "0")) == false,
				IsDeviceOnShleif = innerDriver.addrMask != null && (innerDriver.addrMask == "[8(1)-15(2)];[0(1)-7(255)]" || innerDriver.addrMask == "[0(1)-8(30)]")
			};

			var driverData = DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverId == innerDriver.id && x.IgnoreLevel < 2);
			if (driverData != null)
				driver.DriverType = driverData.DriverType;

			if (innerDriver.options != null)
			{
				driver.DisableAutoCreateChildren = innerDriver.options.Contains("DisableAutoCreateChildren");
				driver.IsZoneLogicDevice = innerDriver.options.Contains("ExtendedZoneLogic");
				driver.CanDisable = innerDriver.options.Contains("Ignorable");
				driver.IsPlaceable = innerDriver.options.Contains("Placeable");
				driver.IsOutDevice = innerDriver.options.Contains("OutDevice");
				driver.IgnoreInZoneState = innerDriver.options.Contains("IgnoreInZoneState");
				driver.IsNotValidateZoneAndChildren = innerDriver.options.Contains("NotValidateZoneAndChildren");
				driver.IsSingleInParent = innerDriver.options.Contains("Single");
				driver.IsSingleInZone = innerDriver.options.Contains("SingleInZone");
				driver.CanMonitoringDisable = innerDriver.options.Contains("CannotDisable") == false;

				driver.CanWriteDatabase = innerDriver.options.Contains("DeviceDatabaseWrite");
				driver.CanReadDatabase = innerDriver.options.Contains("DeviceDatabaseRead");
				driver.CanReadJournal = innerDriver.options.Contains("EventSource") && driver.DriverType != DriverType.IndicationBlock;
				driver.CanSynchonize = innerDriver.options.Contains("HasTimer");
				driver.CanReboot = innerDriver.options.Contains("RemoteReload");
				driver.CanGetDescription = innerDriver.options.Contains("DescriptionString");
				driver.CanSetPassword = innerDriver.options.Contains("PasswordManagement");
				driver.CanUpdateSoft = innerDriver.options.Contains("SoftUpdates");
				driver.CanExecuteCustomAdminFunctions = innerDriver.options.Contains("CustomIOCTLFunctions");
			}
			if (driver.DriverType == DriverType.Exit)
				driver.IsPlaceable = false;

			var metadataClass = Metadata.@class.FirstOrDefault(x => x.clsid == innerDriver.clsid);
			if (metadataClass != null)
			{
				driver.DeviceClassName = metadataClass.param.FirstOrDefault(x => x.name == "DeviceClassName").value;
			}

			driver.CanEditAddress = true;
			if (innerDriver.ar_no_addr != null)
			{
				if (innerDriver.ar_no_addr == "1")
					driver.CanEditAddress = false;

				if (innerDriver.acr_enabled == "1")
					driver.CanEditAddress = false;
			}

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
			if (driver.DriverType == DriverType.BUNS)
				driver.ShleifCount = 2;
			if (driver.DriverType == DriverType.USB_BUNS)
				driver.ShleifCount = 2;

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
			driver.CategoryName = driver.Category.ToDescription();

			driver.DeviceType = DeviceType.FireSecurity;
			if (innerDriver.options != null)
			{
				if (innerDriver.options.Contains("FireOnly"))
					driver.DeviceType = DeviceType.Fire;

				if (innerDriver.options.Contains("SecOnly"))
					driver.DeviceType = DeviceType.Sequrity;

				if (innerDriver.options.Contains("TechOnly"))
					driver.DeviceType = DeviceType.Technoligical;
			}
			driver.DeviceTypeName = driver.DeviceType.ToDescription();

			var driverdata = DriversHelper.DriverDataList.FirstOrDefault(x => (x.DriverId == innerDriver.id));
			if (driverdata != null)
			{
				driver.IsIgnore = driverdata.IgnoreLevel > 1;
				driver.IsAssadIgnore = driverdata.IgnoreLevel > 0;
			}
			else
			{
				return null;
			}

			var allChildren = new List<drvType>();
			foreach (var childDriver in Metadata.drv)
			{
				var childClass = Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
				if (childClass != null && childClass.parent != null && childClass.parent.Any(x => x.clsid == innerDriver.clsid))
				{
					if (childDriver.lim_parent != null && childDriver.lim_parent != innerDriver.id)
						continue;

					allChildren.Add(childDriver);
				}
			}
			try
			{
				driver.Children = new List<Guid>(
					from drvType childInnerDriver in allChildren
					where (DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverId == childInnerDriver.id) != null &&
					DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverId == childInnerDriver.id).IgnoreLevel == 0)
					select new Guid(childInnerDriver.id));
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}

			driver.AvaliableChildren = new List<Guid>(
				from drvType childInnerDriver in allChildren
				where childInnerDriver.acr_enabled != "1"
				select new Guid(childInnerDriver.id));
			if (driver.DisableAutoCreateChildren)
			{
				driver.AutoCreateChildren = new List<Guid>();
			}
			else
			{
				driver.AutoCreateChildren = new List<Guid>(
				from drvType childInnerDriver in allChildren
				where childInnerDriver.acr_enabled == "1"
				select new Guid(childInnerDriver.id));
			}

			driver.CanAddChildren = driver.AvaliableChildren.Count > 0;

			if (innerDriver.child_id != null)
			{
				driver.AutoChild = new Guid(innerDriver.child_id);
				driver.AutoChildCount = int.Parse(innerDriver.child_count);
			}

			driver.CanAutoDetect = allChildren.Any(x => (x.options != null) && (x.options.Contains("CanAutoDetectInstances")));

			driver.Properties = new List<DriverProperty>();
			if (innerDriver.propInfo != null)
			{
				foreach (var internalProperty in innerDriver.propInfo)
				{
					if ((internalProperty.hidden == "1") && (driver.DriverType != DriverType.UOO_TL))
						continue;
					if (internalProperty.caption == "Заводской номер" || internalProperty.caption == "Версия микропрограммы")
						continue;

					var driverProperty = new DriverProperty()
					{
						Name = internalProperty.name,
						Caption = internalProperty.caption,
						ToolTip = internalProperty.hint,
						Default = internalProperty.@default,
						Visible = internalProperty.hidden == "0" && internalProperty.showOnlyInState == "0",
						IsHidden = internalProperty.hidden == "1",
						BlockName = internalProperty.blockName
					};

					if (internalProperty.name.StartsWith("Control$"))
					{
						//driverProperty.Name = internalProperty.name.Replace("Control$", "");
						driverProperty.IsControl = true;
						driver.HasControlProperties = true;
					}

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

							case "Empty":
								driverProperty.DriverPropertyType = DriverPropertyTypeEnum.Empty;
								break;

							default:
								continue;
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
				var codes = new HashSet<string>();
				foreach (var innerState in innerDriver.state)
				{
					if (codes.Add(innerState.code) == false)
					{
						innerState.code += "_" + Guid.NewGuid().ToString();
					}
					if (innerState.name == null)
						continue;
					if (innerState.code == null)
						continue;
					driver.States.Add(new DriverState()
					{
						Id = innerState.id,
						Name = innerState.name,
						AffectChildren = innerState.affectChildren == "1" ? true : false,
						AffectParent = innerState.AffectedParent == "1" ? true : false,
						StateType = (StateType)int.Parse(innerState.@class),
						IsManualReset = innerState.manualReset == "1" ? true : false,
						CanResetOnPanel = innerState.CanResetOnPanel == "1" ? true : false,
						//IsAutomatic = innerState.type == "Auto" ? true : false,
						IsAutomatic = (innerState.code.Contains("AutoOff") || innerState.code.Contains("Auto_Off") || innerState.code.Contains("Auto_off")),
						Code = innerState.code
					});
				}
			}

			if (driver.DriverType == DriverType.Valve)
			{
				AddValveControlProperties(driver);
			}
			return driver;
		}

		static void AddValveControlProperties(Driver driver)
		{
			driver.HasControlProperties = true;
			driver.Properties.Add(new DriverProperty() { IsControl = true, BlockName = "Положение", Name = "BoltClose", Caption = "Закрыть" });
			driver.Properties.Add(new DriverProperty() { IsControl = true, BlockName = "Положение", Name = "BoltStop", Caption = "Стоп" });
			driver.Properties.Add(new DriverProperty() { IsControl = true, BlockName = "Положение", Name = "BoltOpen", Caption = "Открыть" });
			driver.Properties.Add(new DriverProperty() { IsControl = true, BlockName = "Автоматика", Name = "BoltAutoOn", Caption = "Включение автоматики" });
			driver.Properties.Add(new DriverProperty() { IsControl = true, BlockName = "Автоматика", Name = "BoltAutoOff", Caption = "Отключение автоматики" });
			driver.Properties.Add(new DriverProperty() { IsControl = true, BlockName = "Задержка пуска", Name = "BoltOpen", Caption = "Пуск" });
			driver.Properties.Add(new DriverProperty() { IsControl = true, BlockName = "Задержка пуска", Name = "BoltClose", Caption = "Стоп" });
		}
	}
}