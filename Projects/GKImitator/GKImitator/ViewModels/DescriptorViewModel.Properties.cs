using System;
using System.Linq;
using RubezhAPI.GK;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Windows;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		public bool HasParameters { get; private set; }

		public RelayCommand ShowParametersCommand { get; private set; }
		void OnShowParameters()
		{
			SaveCancelDialogViewModel propertiesViewModel = null;
			if (GKBase is GKDevice)
			{
				propertiesViewModel = new DevicePropertiesViewModel(GKBase as GKDevice);
			}
			if (GKBase is GKDirection)
			{
				propertiesViewModel = new DirectionPropertiesViewModel(GKBase as GKDirection);
			}
			if (GKBase is GKDelay)
			{
				propertiesViewModel = new DelayPropertiesViewModel(GKBase as GKDelay);
			}
			if (GKBase is GKGuardZone)
			{
				propertiesViewModel = new GuardZonePropertiesViewModel(GKBase as GKGuardZone);
			}
			if (GKBase is GKDoor)
			{
				propertiesViewModel = new DoorPropertiesViewModel(GKBase as GKDoor);
			}
			if (GKBase is GKPumpStation)
			{
				propertiesViewModel = new PumpStationPropertiesViewModel(GKBase as GKPumpStation);
			}
			if (GKBase is GKMPT)
			{
				propertiesViewModel = new MPTPropertiesViewModel(GKBase as GKMPT);
			}
			if (GKBase is GKCode)
			{
				propertiesViewModel = new CodePropertiesViewModel(GKBase as GKCode);
			}

			if (propertiesViewModel != null && DialogService.ShowModalWindow(propertiesViewModel))
			{
				var journalItem = new ImitatorJournalItem(2, 13, 0, 0);
				AddJournalItem(journalItem);
			}

			InitializeDelays();
		}

		public List<byte> GetParameters(DatabaseType databaseType)
		{
			if (databaseType == DatabaseType.Gk)
			{
				GKBaseDescriptor.Build();
				return GKBaseDescriptor.Parameters;
			}
			else
			{
				KauBaseDescriptor.Build();
				return KauBaseDescriptor.Parameters;
			}
		}

		public void SetParameters(List<byte> bytes)
		{
			var properties = GetPropertiesFromBytes(bytes);
			var device = GKBaseDescriptor.GKBase as GKDevice;
			if (device != null)
			{
				device.Properties = properties;
			}

			var direction = GKBaseDescriptor.GKBase as GKDirection;
			if (direction != null)
			{
				if (properties.Count >= 3)
				{
					direction.Delay = properties[0].Value;
					direction.Hold = properties[1].Value;
					direction.DelayRegime = (DelayRegime)properties[2].Value;
				}
			}

			var delay = GKBaseDescriptor.GKBase as GKDelay;
			if (delay != null)
			{
				if (properties.Count >= 3)
				{
					delay.DelayTime = properties[0].Value;
					delay.Hold = properties[1].Value;
					delay.DelayRegime = (DelayRegime)properties[2].Value;
				}
			}

			var pumpStation = GKBaseDescriptor.GKBase as GKPumpStation;
			if (pumpStation != null)
			{
				if (properties.Count >= 3)
				{
					pumpStation.Delay = properties[0].Value;
					pumpStation.Hold = properties[1].Value;
					pumpStation.DelayRegime = (DelayRegime)properties[2].Value;
				}
			}

			var door = GKBaseDescriptor.GKBase as GKDoor;
			if (door != null)
			{
				if (properties.Count >= 3)
				{
					door.Delay = properties[0].Value;
					door.Hold = properties[1].Value;
				}
			}

			var guardZone = GKBaseDescriptor.GKBase as GKGuardZone;
			if (guardZone != null)
			{
				if (properties.Count >= 3)
				{
					guardZone.SetDelay = properties[0].Value;
					guardZone.ResetDelay = properties[1].Value;
					guardZone.AlarmDelay = properties[2].Value;
				}
			}

			var journalItem = new ImitatorJournalItem(2, 13, 0, 0);
			AddJournalItem(journalItem);
		}

		List<GKProperty> GetPropertiesFromBytes(List<byte> bytes)
		{
			var properties = new List<GKProperty>();

			var binProperties = new List<BinProperty>();
			for (int i = 0; i < bytes.Count / 4; i++)
			{
				byte paramNo = bytes[i * 4];
				ushort paramValue = BytesHelper.SubstructShort(bytes, i * 4 + 1);
				var binProperty = new BinProperty()
				{
					No = paramNo,
					Value = paramValue
				};
				binProperties.Add(binProperty);
			}

			var device = GKBaseDescriptor.GKBase as GKDevice;
			if (device != null)
			{
				foreach (var driverProperty in device.Driver.Properties)
				{
					if (!driverProperty.IsAUParameter)
						continue;

					var binProperty = binProperties.FirstOrDefault(x => x.No == driverProperty.No);
					if (binProperty != null)
					{
						var paramValue = binProperty.Value;
						if (driverProperty.IsLowByte)
						{
							paramValue = (ushort)(paramValue << 8);
							paramValue = (ushort)(paramValue >> 8);
						}
						if (driverProperty.IsHieghByte)
						{
							paramValue = (ushort)(paramValue / 256);
						}
						if (driverProperty.Mask != 0)
						{
							paramValue = (byte)(paramValue & driverProperty.Mask);
						}
						var property = device.DeviceProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
						if (property == null)
						{
							var systemProperty = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
							property = new GKProperty()
							{
								DriverProperty = systemProperty.DriverProperty,
								Name = systemProperty.Name,
								Value = paramValue,
							};
							device.DeviceProperties.Add(property);
						}
						if (property != null)
						{
							property.Value = paramValue;
							property.DriverProperty = driverProperty;
							if (property.DriverProperty.DriverPropertyType == GKDriverPropertyTypeEnum.BoolType)
								property.Value = (ushort)(property.Value > 0 ? 1 : 0);

							properties.Add(property);
						}
					}
					else
						throw new Exception("Неизвестный номер параметра");
				}
			}
			if (binProperties.Count >= 3)
			{
				if (GKBaseDescriptor.DescriptorType == DescriptorType.Direction || GKBaseDescriptor.DescriptorType == DescriptorType.Delay
					|| GKBaseDescriptor.DescriptorType == DescriptorType.GuardZone || GKBaseDescriptor.DescriptorType == DescriptorType.PumpStation
					|| GKBaseDescriptor.DescriptorType == DescriptorType.Door)
				{
					properties.Add(new GKProperty() { Value = binProperties[0].Value });
					properties.Add(new GKProperty() { Value = binProperties[1].Value });
					properties.Add(new GKProperty() { Value = binProperties[2].Value });
				}
			}
			if (GKBaseDescriptor.DescriptorType == DescriptorType.Code && binProperties.Count >= 2)
			{
				properties.Add(new GKProperty() { Value = binProperties[0].Value });
				properties.Add(new GKProperty() { Value = binProperties[1].Value });
			}
			return properties;
		}

		class BinProperty
		{
			public byte No { get; set; }
			public ushort Value { get; set; }
		}

		public bool HasMeasure { get; private set; }

		public RelayCommand ShowMeasureCommand { get; private set; }
		void OnShowMeasure()
		{
			if (GKBase is GKDevice)
			{
				var measureViewModel = new MeasureViewModel(GKBase as GKDevice, MeasureParameterValues);
				if (DialogService.ShowModalWindow(measureViewModel))
				{
					MeasureParameterValues = measureViewModel.MeasureParameterValues; new List<MeasureParameterValue>();
					foreach (var measureParameterValue in MeasureParameterValues)
					{
						if (measureParameterValue.MeasureParameter.No - 1 < AdditionalShortParameters.Count)
						{
							AdditionalShortParameters[measureParameterValue.MeasureParameter.No - 1] = (ushort)measureParameterValue.Value;
						}
					}
				}
			}
		}

		List<MeasureParameterValue> MeasureParameterValues = new List<MeasureParameterValue>();
	}
}