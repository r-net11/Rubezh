using System;
using System.Collections.Generic;
using RubezhAPI.GK;

namespace GKProcessor
{
	public abstract class CommonDatabase
	{
		protected List<GKDevice> Devices { get; set; }
		public DatabaseType DatabaseType { get; protected set; }
		public GKDevice RootDevice { get; protected set; }
		public List<BaseDescriptor> Descriptors { get; set; }
		public GKPim GlobalPim { get; set; }
		public PimDescriptor GlobalPimDescriptor { get; set; }

		public CommonDatabase()
		{
			Devices = new List<GKDevice>();
			Descriptors = new List<BaseDescriptor>();
		}

		protected void RestructCollection(List<GKDevice> devices)
		{
			var gkRelays = devices.FindAll(x => x.DriverType == GKDriverType.GKRele);
			var gkRelaysIndecses = new List<Tuple<GKDevice, int>>();
			gkRelays.ForEach(x => gkRelaysIndecses.Add(new Tuple<GKDevice, int>(x, devices.IndexOf(x) - 5)));
			gkRelays.ForEach(x => devices.Remove(x));
			gkRelaysIndecses.ForEach(x => devices.Insert(x.Item2, x.Item1));
		}

		public abstract void BuildObjects();

		public IEnumerable<DescriptorError> Check()
		{
			foreach (var descriptor in Descriptors)
			{
				if(!descriptor.Formula.CheckStackOverflow())
					yield return new DescriptorError(descriptor, "Ошибка глубины стека дескриптора");

				var type = BytesHelper.SubstructShort(descriptor.AllBytes, 0);
				int offsetPosition = 0;
				if(DatabaseType == DatabaseType.Kau)
				{
					var physicalAddress = BytesHelper.SubstructShort(descriptor.AllBytes, 2);
					offsetPosition = 4;
				}
				if (DatabaseType == DatabaseType.Gk || DatabaseType == DatabaseType.Mirror)
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
				if (DatabaseType == DatabaseType.Gk || DatabaseType == DatabaseType.Mirror)
				{
					inputDependencesCount = BytesHelper.SubstructShort(descriptor.AllBytes, offsetPosition + 2);
					for (int i = 0; i < inputDependencesCount; i++)
					{
						var no = BytesHelper.SubstructShort(descriptor.AllBytes, offsetPosition + 4 + i * 2);
						if (no == 0)
							yield return new DescriptorError(descriptor, "Значение входной зависимости равно 0");
						if (no > Descriptors.Count)
							yield return new DescriptorError(descriptor, "Значение входной зависимости больше количества компонентов");
					}
					outputDependencesPosition = offsetPosition + 2 + 2 + inputDependencesCount * 2;
				}

				var outputDependencesCount = BytesHelper.SubstructShort(descriptor.AllBytes, outputDependencesPosition);
				for (int i = 0; i < outputDependencesCount; i++)
				{
					var no = BytesHelper.SubstructShort(descriptor.AllBytes, outputDependencesPosition + 2 + i * 2);
					if (no == 0)
						yield return new DescriptorError(descriptor, "Значение выходной зависимости равно 0");
					if (no > Descriptors.Count)
						yield return new DescriptorError(descriptor, "Значение выходной зависимости больше количества компонентов");
				}

				var formulaPosition = outputDependencesPosition + 2 + outputDependencesCount * 2;
				var formulaLenght = offsetToParameters - formulaPosition;
				if (formulaLenght == 0)
					yield return new DescriptorError(descriptor, "Пустой дескриптор");

				for (int i = 0; i < formulaLenght / 4; i++)
				{
					var f1 = descriptor.AllBytes[formulaPosition + i * 4 + 0];
					var f2 = descriptor.AllBytes[formulaPosition + i * 4 + 1];
					var f3 = BytesHelper.SubstructShort(descriptor.AllBytes, formulaPosition + i * 4 + 2);
					if(f1 == (ushort)FormulaOperationType.GETBIT || f1 == (ushort)FormulaOperationType.GETBYTE || f1 == (ushort)FormulaOperationType.GETWORD || f1 == (ushort)FormulaOperationType.PUTBIT || f1 == (ushort)FormulaOperationType.PUTBYTE || f1 == (ushort)FormulaOperationType.PUTWORD)
					{
						if(f3 == 0)
							yield return new DescriptorError(descriptor, "Не найден дескриптор в логике");
					}
				}

				var lastFormulaOperationType = descriptor.AllBytes[formulaPosition + formulaLenght - 4];
				if (lastFormulaOperationType != (int)FormulaOperationType.END)
					yield return new DescriptorError(descriptor, "Логика должна заканчиваться операцией End");

				var parametersCount = BytesHelper.SubstructShort(descriptor.AllBytes, offsetToParameters);
				if (descriptor.AllBytes.Count != offsetToParameters + 2 + parametersCount * 4)
					yield return new DescriptorError(descriptor, "Ошибка длины дескриптора");
			}
		}
	}

	public class DescriptorError
	{
		public DescriptorError(BaseDescriptor baseDescriptor, string error)
		{
			BaseDescriptor = baseDescriptor;
			Error = error;
		}

		public BaseDescriptor BaseDescriptor { get; set; }
		public string Error { get; set; }
	}
}