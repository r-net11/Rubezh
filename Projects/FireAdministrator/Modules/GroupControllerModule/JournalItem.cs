using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule
{
	public class JournalItem
	{
		public int No { get; set; }
		public DateTime DateTime { get; set; }
		public byte Day { get; set; }
		public byte Month { get; set; }
		public byte Year { get; set; }
		public byte Hour { get; set; }
		public byte Minute { get; set; }
		public byte Second { get; set; }
		public short Address { get; set; }
		public byte Source { get; set; }
		public byte Code { get; set; }

		public JournalItem(List<byte> bytes)
		{
			No = 1 * bytes[0] + 256 * bytes[1] + 256 * 256 * bytes[2] + 256 * 256 * 256 * bytes[3];
			Day = bytes[4];
			Month = bytes[5];
			Year = bytes[6];
			Hour = bytes[7];
			Minute = bytes[8];
			Second = bytes[9];
			DateTime = DateTime.Now;
			//DateTime = new DateTime(year, month, day, hour, minute, second);
			Address = (short)(bytes[10] + 256 * bytes[11]);
			Source = bytes[12];
			Code = bytes[13];
		}

		public override string ToString()
		{
			return "Номер: " + No.ToString() + "\n" +
				"Время: " + Day.ToString() + "/" + Month.ToString() + "/" + Year.ToString() + " " +
				Hour.ToString() + ":" + Minute.ToString() + ":" + Second.ToString() + "\n" +
				"Адрес: " + Address.ToString() + "\n" +
				"Источник: " + Source.ToString() + "\n" +
				"Код: " + Code.ToString();
		}
	}
}