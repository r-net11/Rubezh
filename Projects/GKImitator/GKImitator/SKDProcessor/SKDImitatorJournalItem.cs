using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GKImitator.Processor;

namespace GKImitator
{
	public class SKDImitatorJournalItem
	{
		public int No { get; set; }
		public int Source { get; set; }
		public int Code { get; set; }
		public int CardNo { get; set; }

		public List<byte> ToBytes()
		{
			var result = new List<byte>();
			result.AddRange(SKDImitatorProcessor.IntToBytes(No));
			result.Add((byte)Source);
			result.Add((byte)Code);
			result.AddRange(SKDImitatorProcessor.IntToBytes(CardNo));
			return result;
		}
	}
}