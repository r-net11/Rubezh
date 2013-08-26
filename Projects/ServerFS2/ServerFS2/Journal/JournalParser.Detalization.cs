using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Api;
using Rubezh2010;
using System.Collections;
using Common;

namespace ServerFS2
{
	public partial class JournalParser
	{
		string GetDetalization()
		{
			var result = "";
			result += GetDetalizationForAM1_O();
			result += GetDetalizationForConnectionLost();
			result += GetDetalizationForDevice();
			result += GetDetalizationForEvent();

			if (FSInternalJournal.EventCode == 0x0D && FSInternalJournal.State == 0x20)
			{
				result += "база (сигнатура) повреждена или отсутствует\n";
			}

			if (!string.IsNullOrEmpty(FS2JournalItem.DeviceName))
			{
				result += "Устройство: " + FS2JournalItem.DeviceName + "\n";
			}
			if (!string.IsNullOrEmpty(FS2JournalItem.ZoneName))
			{
				result += "Зона: " + FS2JournalItem.ZoneName + "\n";
			}

			if (result.EndsWith("\n"))
				result = result.Remove(result.Length - 1, 1);

			return result;
		}

		string GetDetalizationForDevice()
		{
			string result = "";
			try
			{
				if (FS2JournalItem.DeviceUID != Guid.Empty && FSInternalJournal.DeviceType != 0)
				{
					var stringTableType = MetadataHelper.GetDeviceTableNo(FS2JournalItem.Device);
					if (stringTableType != null && MetadataEvent != null && MetadataEvent.detailsFor != null)
					{
						var metadataDetailsFor = MetadataEvent.detailsFor.FirstOrDefault(x => x.tableType == stringTableType);
						if (metadataDetailsFor != null)
						{
							var metadataDictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == metadataDetailsFor.dictionary);
							var stateBitArray = new BitArray(new int[] { FSInternalJournal.State });
							var stateValue = "0x" + FSInternalJournal.State.ToString("X2");
							foreach (var bit in metadataDictionary.bit)
							{
								if (bit.val == stateValue && bit.no == null)
								{
									result += bit.value + "\n";
									break;
								}
								if (bit.no != null && stateBitArray.Get(Convert.ToInt32(bit.no)))
								{
									result += bit.value + "\n";
								}
								if (bit.no != null && bit.no.Contains('-'))
								{
									var values = bit.no.Split('-');
									if (values.Count() == 2)
									{
										int parsedInt = 0;
										if (Int32.TryParse(values[0], out parsedInt))
										{
											var startBit = parsedInt;
											if (Int32.TryParse(values[1], out parsedInt))
											{
												var endBit = parsedInt;

												var bitsValue = 0;
												for (int i = startBit; i <= endBit; i++)
												{
													var bitValue = (stateBitArray[i] ? 1 : 0) << (i - startBit);
													bitsValue += bitValue;
												}
												if (bitsValue.ToString() == bit.value)
												{
													result += bit.value + "\n";
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch
			{
				Logger.Error("JournalParser.GetEventDetalization");
			}
			return result;
		}

		string GetDetalizationForAM1_O()
		{
			if (FSInternalJournal.EventCode != 0x83)
			{
				if (FSInternalJournal.DeviceType == 0x34)
				{
					switch (Bytes[22])
					{
						case 0:
							return "Тип датчика: Без типа\n";
						case 1:
							return "Тип датчика: Стекло\n";
						case 2:
							return "Тип датчика: Дверь\n";
						case 3:
							return "Тип датчика: Объем\n";
						case 4:
							return "Тип датчика: Тревожная кнопка\n";
					}
				}
			}
			return "";
		}

		string GetDetalizationForEvent()
		{
			switch (FSInternalJournal.EventCode)
			{
				case 0x01:
					return "Пожарная тревога\n";
				case 0x31:
				case 0x32:
				case 0x45:
					if (Bytes[21] == 0)
						return "команда с компьютера\n";
					break;

				case 0x28:
					if (FS2JournalItem.DeviceCategory == 0x00)
						FS2JournalItem.DeviceCategory = 0x75;

					switch (Bytes[24])
					{
						case 0:
							{
								var result = "команда с компьютера\n";
								if (Bytes[23] == 0)
									result += "через USB\n";
								else
									result += "через канал МС " + Bytes[23] + "\n";
								return result;
							}
						case 3:
							return "Прибор: Рубеж-БИ Адрес:" + Bytes[23] + "\n";
						case 7:
							return "Прибор: Рубеж-ПДУ Адрес:" + Bytes[23] + "\n";
						case 9:
							return "Прибор: Рубеж-ПДУ-ПТ Адрес:" + Bytes[23] + "\n";
						case 100:
							return "Устройство: МС-3 Адрес:" + Bytes[23] + "\n";
						case 101:
							return "Устройство: МС-4 Адрес:" + Bytes[23] + "\n";
						case 102:
							return "Устройство: УОО-ТЛ Адрес:" + Bytes[23] + "\n";
					}
					break;
			}
			return "";
		}

		string GetDetalizationForConnectionLost()
		{
			if (FSInternalJournal.EventCode == 0x85)
			{
				switch (FSInternalJournal.DeviceType)
				{
					case 3:
						return "Прибор: Рубеж-БИ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 7:
						return "Прибор: Рубеж-ПДУ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 100:
						return "Устройство: МС-3 Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 101:
						return "Устройство: МС-4 Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
					case 102:
						return "Устройство: УОО-ТЛ Адрес:" + (FSInternalJournal.ShleifNo - 1).ToString() + "\n";
				}
			}
			return "";
		}
	}
}
