using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using FiresecAPI.Models;

namespace DeviveModelManager
{
	public class TreeItem
	{
		public TreeItem()
		{
			Children = new ObservableCollection<TreeItem>();
		}

		public string Name { get; set; }
		public Assad.modelInfoType ModelInfo { get; set; }
		public ObservableCollection<TreeItem> Children { get; set; }
		Driver _driver;

		public void SetDriver(Driver driver)
		{
			_driver = driver;
			Name = _driver.Name;

			ModelInfo = new Assad.modelInfoType()
			{
				name = _driver.Name,
				type1 = "rubezh." + ViewModel.StaticVersion + "." + _driver.UID.ToString(),
				model = "1.0",
				@event = AddEvents().ToArray(),
				command = AddCommands().ToArray(),
				param = AddParameters().ToArray(),
				state = AddStates().ToArray()
			};
		}

		List<Assad.modelInfoTypeEvent> AddEvents()
		{
			var events = new List<Assad.modelInfoTypeEvent>();
			foreach (var state in CommonStatesHelper.States)
			{
				events.Add(new Assad.modelInfoTypeEvent() { @event = state });
			}
			return events;
		}

		List<Assad.modelInfoTypeCommand> AddCommands()
		{
			var commands = new List<Assad.modelInfoTypeCommand>();
			foreach (var state in _driver.States)
			{
				if (state.IsManualReset)
				{
					commands.Add(new Assad.modelInfoTypeCommand() { command = "Сброс " + state.Name });
				}
			}
			return commands;
		}


		List<Assad.modelInfoTypeParam> AddParameters()
		{
			var parameters = new List<Assad.modelInfoTypeParam>();
			if (_driver.HasAddress)
			{
				parameters.Add(new Assad.modelInfoTypeParam() { param = "Адрес", type = "edit" });
			}
			return parameters;
		}

		List<Assad.modelInfoTypeState> AddStates()
		{
			var States = new List<Assad.modelInfoTypeState>();
			var AssadState = new Assad.modelInfoTypeState();
			AssadState.state = "Состояние";
			var StateValues = new List<Assad.modelInfoTypeStateValue>();
			foreach (var state in CommonStatesHelper.States)
			{
				StateValues.Add(new Assad.modelInfoTypeStateValue() { value = state });
			}
			AssadState.value = StateValues.ToArray();
			States.Add(AssadState);

			States.Add(new Assad.modelInfoTypeState() { state = "Примечание" });

			if (_driver.IsZoneDevice)
			{
				States.Add(new Assad.modelInfoTypeState() { state = "Зона" });
			}
			if (_driver.IsZoneLogicDevice)
			{
				States.Add(new Assad.modelInfoTypeState() { state = "Настройка включения по состоянию зон" });
			}

			foreach (var propInfo in _driver.Properties)
			{
				var customParam = new Assad.modelInfoTypeState();
				if (propInfo.IsHidden == false)
				{
					if (!string.IsNullOrEmpty(propInfo.Caption))
					{
						customParam.state = propInfo.Caption;
						if (propInfo.Caption == "Адрес")
						{
							customParam.state = "Адрес USB устройства в сети RS-485";
						}
						States.Add(customParam);
					}
				}
			}

			var AssadConfigurationState = new Assad.modelInfoTypeState();
			AssadConfigurationState.state = "Конфигурация";
			var ConfigurationStateValues = new List<Assad.modelInfoTypeStateValue>();
			ConfigurationStateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Норма" });
			ConfigurationStateValues.Add(new Assad.modelInfoTypeStateValue() { value = "Ошибка" });
			AssadConfigurationState.value = ConfigurationStateValues.ToArray();
			States.Add(AssadConfigurationState);
			foreach (var paramInfo in _driver.Parameters)
			{
				if (paramInfo.Visible)
				{
					States.Add(new Assad.modelInfoTypeState() { state = paramInfo.Caption });
				}
			}

			return States;
		}

		public string ModelInfoXml
		{
			get
			{
				var serializer = new XmlSerializer(typeof(Assad.modelInfoType));
				var memoryStream = new MemoryStream();
				serializer.Serialize(memoryStream, ModelInfo);
				byte[] bytes = memoryStream.ToArray();
				memoryStream.Close();
				return Encoding.UTF8.GetString(bytes);
			}
		}
	}
}