using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public abstract class CommonDatabase
	{
		protected List<GKCode> Codes { get; set; }
		protected List<GKDevice> Devices { get; set; }
		protected List<GKZone> Zones { get; set; }
		protected List<GKGuardZone> GuardZones { get; set; }
		protected List<GKDirection> Directions { get; set; }
		protected List<GKPumpStation> PumpStations { get; set; }
		protected List<GKMPT> MPTs { get; set; }
		public List<GKDelay> Delays { get; protected set; }
		public List<GKPim> Pims { get; protected set; }

		public DatabaseType DatabaseType { get; protected set; }
		public GKDevice RootDevice { get; protected set; }
		public List<BaseDescriptor> Descriptors { get; set; }

		public CommonDatabase()
		{
			Codes = new List<GKCode>();
			Devices = new List<GKDevice>();
			Zones = new List<GKZone>();
			GuardZones = new List<GKGuardZone>();
			Directions = new List<GKDirection>();
			PumpStations = new List<GKPumpStation>();
			MPTs = new List<GKMPT>();
			Delays = new List<GKDelay>();
			Pims = new List<GKPim>();
			Descriptors = new List<BaseDescriptor>();
		}

		public void AddDelay(GKDelay delay)
		{
			if (!Delays.Contains(delay))
			{
				if (DatabaseType == DatabaseType.Gk)
				{
					delay.GkDatabaseParent = RootDevice;
				}
				else
				{
					delay.KauDatabaseParent = RootDevice;
					delay.IsLogicOnKau = true;
					delay.GkDatabaseParent = RootDevice.GKParent;
				}
				Delays.Add(delay);
			}
		}

		public void AddPim(GKPim pim)
		{
			if (!Pims.Contains(pim) && pim != null)
			{
				if (DatabaseType == DatabaseType.Gk)
				{
					pim.GkDatabaseParent = RootDevice;
				}
				else
				{
					pim.KauDatabaseParent = RootDevice;
					pim.GkDatabaseParent = RootDevice.GKParent;
				}
				Pims.Add(pim);
			}
		}

		public abstract void BuildObjects();

		public string Check()
		{
			foreach (var descriptor in Descriptors)
			{
				var type = BytesHelper.SubstructShort(descriptor.AllBytes, 0);
				int offsetPosition = 0;
				if(DatabaseType == DatabaseType.Kau)
				{
					var physicalAddress = BytesHelper.SubstructShort(descriptor.AllBytes, 2);
					offsetPosition = 4;
				}
				if (DatabaseType == DatabaseType.Gk)
				{
					var controllerAddress = BytesHelper.SubstructShort(descriptor.AllBytes, 2);
					var addressOnController = BytesHelper.SubstructShort(descriptor.AllBytes, 4);
					var physicalAddress = BytesHelper.SubstructShort(descriptor.AllBytes, 6);
					var description = BytesHelper.BytesToStringDescription(descriptor.AllBytes, 8);
					offsetPosition = 40;
				}
				var offsetToParameters = BytesHelper.SubstructShort(descriptor.AllBytes, offsetPosition);

				var inputDependencesCount = 0;
				var outputDependencesPosition = offsetPosition + 2;
				if(DatabaseType == DatabaseType.Gk)
				{
					inputDependencesCount = BytesHelper.SubstructShort(descriptor.AllBytes, offsetPosition + 2);
					for (int i = 0; i < inputDependencesCount; i++)
					{
						var no = BytesHelper.SubstructShort(descriptor.AllBytes, offsetPosition + 4 + i * 2);
						if (no == 0)
							return "Значение входной зависимости равно 0";
						if (no > Descriptors.Count)
							return "Значение входной зависимости больше количества компонентов";
					}
					outputDependencesPosition = offsetPosition + 2 + 2 + inputDependencesCount * 2;
				}

				var outputDependencesCount = BytesHelper.SubstructShort(descriptor.AllBytes, outputDependencesPosition);
				for (int i = 0; i < outputDependencesCount; i++)
				{
					var no = BytesHelper.SubstructShort(descriptor.AllBytes, outputDependencesPosition + 2 + i * 2);
					if (no == 0)
						return "Значение входной зависимости равно 0";
					if (no > Descriptors.Count)
						return "Значение входной зависимости больше количества компонентов";
				}

				var formulaPosition = outputDependencesPosition + 2 + outputDependencesCount * 2;
				var formulaLenght = offsetToParameters - formulaPosition;
				if (formulaLenght == 0)
					return "Пустой дескриптор";

				for (int i = 0; i < formulaLenght / 4; i++)
				{
					var f1 = descriptor.AllBytes[formulaPosition + i * 4 + 0];
					var f2 = descriptor.AllBytes[formulaPosition + i * 4 + 1];
					var f3 = descriptor.AllBytes[formulaPosition + i * 4 + 2];
					var f4 = descriptor.AllBytes[formulaPosition + i * 4 + 3];
				}

				var lastFormulaOperationType = descriptor.AllBytes[formulaPosition + formulaLenght - 4];
				if (lastFormulaOperationType != (int)FormulaOperationType.END)
					return "Логика должна заканчиваться операцией End";

				var parametersCount = BytesHelper.SubstructShort(descriptor.AllBytes, offsetToParameters);
				if (descriptor.AllBytes.Count != offsetToParameters + 2 + parametersCount * 4)
					return "Ошибка длины дескриптора";
			}
			return null;
		}
	}
}