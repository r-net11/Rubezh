using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GKImitator.Processor;

namespace GKImitator
{
	public class SKDImitatorJournalItem
	{
		public SKDImitatorJournalItem()
		{
			DateTime = DateTime.Now.AddSeconds(-1);
		}

		public int No { get; set; }
		public DateTime DateTime { get; set; }
		public int Source { get; set; }
		public int Address { get; set; }
		public int NameCode { get; set; }
		public int DescriptionCode { get; set; }
		public int CardSeries { get; set; }
		public int CardNo { get; set; }

		public List<byte> ToBytes()
		{
			var result = new List<byte>();
			result.AddRange(SKDImitatorProcessor.IntToBytes(No));
			result.Add((byte)(DateTime.Year-2000));
			result.Add((byte)DateTime.Month);
			result.Add((byte)DateTime.Day);
			result.Add((byte)DateTime.Hour);
			result.Add((byte)DateTime.Minute);
			result.Add((byte)DateTime.Second);
			result.Add((byte)Source);
			result.Add((byte)Address);
			result.Add((byte)NameCode);
			result.Add((byte)DescriptionCode);
			result.AddRange(SKDImitatorProcessor.IntToBytes(CardSeries));
			result.AddRange(SKDImitatorProcessor.IntToBytes(CardNo));
			return result;
		}
	}
}