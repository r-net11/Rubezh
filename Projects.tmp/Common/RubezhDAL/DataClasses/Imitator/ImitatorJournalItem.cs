using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace RubezhDAL.DataClasses
{
	public class ImitatorJournalItem
	{
		public ImitatorJournalItem()
		{
			UID = Guid.NewGuid();
			DateTime = DateTime.Now;
			ObjectDeviceType = 0;
			ObjectDeviceAddress = 0;
		}

		public ImitatorJournalItem(byte source, byte nameCode, byte descriptionCode, byte yesNoCode)
			: this()
		{
			Source = source;
			NameCode = nameCode;
			DescriptionCode = descriptionCode;
			YesNoCode = yesNoCode;
		}

		[Key]
		public Guid UID { get; set; }
		public int GkNo { get; set; }
		public int GkObjectNo { get; set; }
		public int UNUSED_KauNo { get; set; }
		public DateTime DateTime { get; set; }
		public short UNUSED_KauAddress { get; set; }
		public byte Source { get; set; } // Controller = 0, Device = 1, Object = 2
		public byte NameCode { get; set; }
		public byte YesNoCode { get; set; }
		public byte DescriptionCode { get; set; }
		public short ObjectNo { get; set; }
		public short ObjectDeviceType { get; set; }
		public short ObjectDeviceAddress { get; set; }
		public int ObjectFactoryNo { get; set; }
		public int ObjectState { get; set; }

		public List<byte> ToBytes()
		{
			var result = new List<byte>();
			result.AddRange(BitConverter.GetBytes(GkNo));
			result.AddRange(BitConverter.GetBytes(GkObjectNo));
			for (int i = 0; i < 24; i++)
			{
				result.Add(0);
			}
			result.AddRange(BitConverter.GetBytes(UNUSED_KauNo));

			result.Add((byte)DateTime.Day);
			result.Add((byte)DateTime.Month);
			result.Add((byte)(DateTime.Year - 2000));
			result.Add((byte)DateTime.Hour);
			result.Add((byte)DateTime.Minute);
			result.Add((byte)DateTime.Second);

			result.AddRange(BitConverter.GetBytes(UNUSED_KauAddress));
			result.Add(Source);
			result.Add(NameCode);

			result.Add(YesNoCode);
			result.Add(DescriptionCode);

			result.AddRange(BitConverter.GetBytes((short)0));
			result.AddRange(BitConverter.GetBytes(ObjectNo));
			result.AddRange(BitConverter.GetBytes(ObjectDeviceType));
			result.AddRange(BitConverter.GetBytes(ObjectDeviceAddress));
			result.AddRange(BitConverter.GetBytes(ObjectFactoryNo));
			result.AddRange(BitConverter.GetBytes(ObjectState));

			return result;
		}
	}
}