using ResursAPI.ParameterNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class Mercury203CounterCreator
	{
		public static Driver Create()
		{
			var driver = new Driver(new Guid("621C2188-C164-49DE-AC54-A5D6BBA95332"));
			driver.DriverType = DriverType.Mercury203Counter;
			driver.DeviceType = DeviceType.Counter;
			driver.CanEditTariffType = false;
			driver.DefaultTariffType = TariffType.Electricity;
			driver.DriverParameters.Add(new DriverParameter(new Guid("9A05706F-F8A7-4539-8D39-DEC383831274"))
			{
				Name = ParameterNamesMercury203.GADDR,
				Description = "Групповой адрес",
				ParameterType = ParameterType.Int,
				IsReadOnly = true,
				Number = 0,
				IntMinValue = 1
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("22566EA7-7A66-4C43-B615-AEBDF211DA16"))
			{
				Name = ParameterNamesMercury203.DateTime,
				Description = "Дата и время",
				ParameterType = ParameterType.DateTime,
				IsReadOnly = true,
				Number = 1
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("3FDBEBBC-1FE4-4EDF-A4D9-2E476A330D61"))
			{
				Name = ParameterNamesMercury203.PowerLimit,
				Description = "Лимит мощности",
				ParameterType = ParameterType.Double,
				IsReadOnly = true,
				Number = 2,
				DoubleMinValue = 0.001
			});
			driver.DriverParameters.Add(new DriverParameter(new Guid("77C3C079-FD03-419E-B80F-4D101E855358"))
			{
				Name = ParameterNamesMercury203.PowerLimitPerMonth,
				Description = "Месячный лимит мощности",
				ParameterType = ParameterType.Double,
				IsReadOnly = true,
				Number = 3,
				DoubleMinValue = 0.001
			});
			return driver;
		}
	}
}
