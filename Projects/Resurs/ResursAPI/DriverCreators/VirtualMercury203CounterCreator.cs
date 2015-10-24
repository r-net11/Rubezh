using ResursAPI.ParameterNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class VirtualMercury203CounterCreator
	{
		public static Driver Create()
		{
			var driver = new Driver(new Guid("5AFE758E-5659-4D28-B3E0-BF50F360851E"));
			driver.DriverType = DriverType.VirtualMercury203Counter;
			driver.DeviceType = DeviceType.Counter;
			driver.CanEditTariffType = false;
			driver.DefaultTariffType = TariffType.Electricity;
			var realDriver = DriversConfiguration.GetDriver(DriverType.Mercury203Counter);
			driver.DriverParameters.AddRange(realDriver.DriverParameters);
			driver.Commands.AddRange(realDriver.Commands);
			driver.Commands.Add(new DeviceCommand(new Guid("7DB04C01-18D3-4F65-BC39-9BC93CAB6AE1")) 
			{ 
				Name = CommandNames.CommandNamesMercury203Virtual.SetCommunicationError, 
				Description = "Ошибка доступа" 
			});
			driver.Commands.Add(new DeviceCommand(new Guid("7B7939E7-E820-4DBA-89E3-1A0DBEC7A825"))
			{
				Name = CommandNames.CommandNamesMercury203Virtual.ResetCommunicationError,
				Description = "Сброс ошибки доступа"
			});
			driver.Commands.Add(new DeviceCommand(new Guid("636C5E61-481B-43B1-B5E6-5C03B260471E"))
			{
				Name = CommandNames.CommandNamesMercury203Virtual.SetConfigurationError,
				Description = "Ошибка конфигурации"
			});
			driver.Commands.Add(new DeviceCommand(new Guid("FABC15C1-58D3-4266-9BF4-D9D8227212E0"))
			{
				Name = CommandNames.CommandNamesMercury203Virtual.ResetConfigurationError,
				Description = "Сброс ошибки конфигурации"
			});
			driver.Commands.Add(new DeviceCommand(new Guid("1DEC3D95-3FAE-47AD-9872-3B77722D6E67"))
			{
				Name = CommandNames.CommandNamesMercury203Virtual.SetRtcError,
				Description = "Ошибка часов"
			});
			driver.Commands.Add(new DeviceCommand(new Guid("57FA3989-FFD7-4B64-BAD0-5F15F78BD87D"))
			{
				Name = CommandNames.CommandNamesMercury203Virtual.ResetRtcError,
				Description = "Сброс ошибки часов"
			}); 
			return driver;
		}
	}
}
